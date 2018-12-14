<?php

class Enbilulu 
{

    const INIT = "
       create table droplets (
           id integer PRIMARY KEY AUTOINCREMENT, 
           created_at datetime default (strftime('%Y-%m-%dT%H:%M:%SZ', 'now')), 
           payload text
        );
    ";
    
    const INSERT_DATA = "
        insert into droplets (payload)
        values (:payload)
    ";
    
    const GET_POINT_BEFORE_X = "
        select id from droplets where id < :id order by id desc limit 1
    ";

    const GET_POINT_AFTER_X = "
        select id from droplets where id > :id order by id limit 1
    ";

    const GET_EARLIEST_POINT = "
        select min(id) as id from droplets
    ";

    const GET_LAST_POINT = "
        select max(id) as id from droplets
    ";

    const GET_POINTS = "
        select id, created_at, payload
        from droplets
        where id >= :id
        limit :limit
    ";

    function create($brook)
    {
        if (exists($brook))
        {
            throw new Exception("Stream already exists.") ;
        }  

        $db = $this->get_db($brook);
        $db->exec(self::INIT);
    }

    function exists($brook)
    {
        return file_exists($brook);
    }

    function put_record($brook, $data)
    {
        $db = $this->get_db($brook);
        $result = $this->execute($db, 
            self::INSERT_DATA,
            Array(':payload'=> json_encode($data))
        );
        
        return $db->lastInsertRowID();
    }

    function get_records($brook, $point, $count)
    {
        $db = $this->get_db($brook);
        
        $points = $this-execute($db, 
            self::GET_POINTS,
            Array(
                ':id'=>$point,
                ':limit'=>$count
            )
        );
        
        $data = array();

        while ($p = $points->fetchArray())
        {
            $item = (object)array(
                'sequence_number'=>(int)$p["id"],
                'data'=>$p["payload"],
                'created_at'=>DateTime::createFromFormat(DateTime::ISO8601, $p["created_at"])
            );

            array_push($data, $item);
        }

        if (count($data) > 0)
        {
            end($data);
            $key = key($data);
            reset($data);

            $last_point = $data[$key]->sequence_number;
            $next_point=$this->get_stream_point($brook, Array('type'=>'after_point', 'starting_point'=>$last_point));;
            $milliseconds_behind = $this->TotalMillisecondsDiff(
                date_diff(
                    $data[$key]->created_at, 
                    new \DateTime("now", new \DateTimeZone("UTC"))
                )
            );
        }
        else
        {
            $last_point = null;
            $next_point = $p;
            $milliseconds_behind = 0;
        }

        return (object)array(
            'last_point'=>$last_point,
            'next_point'=>$next_point,
            'milliseconds_behind'=>$milliseconds_behind,
            'records'=>$data
        );
    }

    function get_stream_point($brook, $config)
    {
        $db = $this->get_db($brook);

        switch ($config['type']) {
            case 'after_point':
                $point = $config['starting_point'];
                $r = $this->execute($db, 
                    self::GET_POINT_AFTER_X, 
                    Array(':id'=>$point)
                );

                $id = $this->get_id_from_rs($r);
                
                if ($id == null)
                {
                    $id = $point+1;
                }
                
                break;
            case 'before_point':
                $point = $config['starting_point'];
                $r = $this->execute($db, 
                    self::GET_POINT_BEFORE_X, 
                    Array(':id'=>$point)
                );
                
                $id = $this->get_id_from_rs($r);
                
                if ($id == null)
                {
                    $id = $point;
                }
                break;
            case 'start':
                $r = $db->query(self::GET_EARLIEST_POINT);
                $id = $this->get_id_from_rs($r);
                if ($id == null)
                {
                    $id = 0;
                }
                break;
            default:
                $r = $db->query(self::GET_LAST_POINT);
                $id = $this->get_id_from_rs($r);
                if ($id == null)
                {
                    $id = 0;
                }
                break;
            }

        return $id;
    }

    private function get_db($name)
    {
        if (!exists($name))
        {
            throw new Exception("Stream does not exist.");
        }
        return new SQLite3($name);
    }

    private function get_id_from_rs($rs)
    {
        if ($rs == null)
        {
            return null;
        }
        $record = $rs->fetchArray();
        return (int)$record["id"];
    }

    private function execute($db, $sql, $params)
    {
        $stmt = $db->prepare($sql);
        foreach ($params as $key => $value){
            $stmt->bindValue($key, $value);
        }
        return $stmt->execute();
    }

    private function TotalMillisecondsDiff($dateInterval)
    {
        $di = $dateInterval;
        return ((((($di->days)
         * 24 + $di->h)
         * 60 + $di->m)
         * 60 + $di->s)
         * 1 + $di->f)
         * 1000;
    }
}
?>
