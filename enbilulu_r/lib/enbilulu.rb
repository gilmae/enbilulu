require "sqlite3"
require "json"

module Enbilulu
    INIT = <<-eos
       create table droplets (
           id integer PRIMARY KEY AUTOINCREMENT, 
           created_at datetime default (strftime('%Y-%m-%dT%H:%M:%SZ', 'now')), 
           payload text
        );
    eos

    INSERT_DATA = <<-eos
        insert into droplets (payload)
        values (?)
    eos

    GET_POINT_BEFORE_X = <<-eos
        select id from droplets where id < ? order by id desc limit 1
    eos

    GET_POINT_AFTER_X = <<-eos
        select id from droplets where id > ? order by id limit 1
    eos

    GET_EARLIEST_POINT = <<-eos
        select min(id) as id from droplets
    eos

    GET_LAST_POINT = <<-eos
        select max(id) as id from droplets
    eos

    GET_POINTS = <<-eos
        select id, created_at, payload
        from droplets
        where id >= ?
        limit ?
    eos

    def create brook
        raise ArgumentError.new("Stream already exists.") if exists?(brook)  

        db = SQLite3::Database.new brook
        db.execute INIT
    end

    def exists? brook
        File.exists?(brook) 
    end

    def put_record brook, data
        db = get_db brook
        db.execute(INSERT_DATA, data.to_json)

        db.last_insert_row_id()
    end

    def get_stream_point brook, config
        db = get_db brook
        db.results_as_hash = true

        case config[:type]
        when :after_point
            point = config[:starting_point]
            r = db.execute GET_POINT_AFTER_X, point
            id = get_id_from_rs(r)
            return (point+1) if id == nil
            return id
        when :before_point
            point = config[:starting_point]
            r = db.execute GET_POINT_BEFORE_X, point
            id = get_id_from_rs(r)
            return (point) if id == nil
            return id
        when :start
            r = db.execute GET_EARLIEST_POINT
            id = get_id_from_rs(r)
            return 0 if id == nil
            return id
        else
            r = db.execute GET_LAST_POINT
            id = get_id_from_rs(r)
            return 0 if id == nil
            return id       
        end
    end

    def get_records brook, point, count
        db = get_db brook
        db.results_as_hash = true
        points = db.execute(GET_POINTS, point, count)

        data = points.map do |p|
            {
                :sequence_number=>p["id"].to_i,
                :data => p["payload"],
                :created_at => Time.parse(p["created_at"])
            }
        end

        if data && data.length > 0
            last_point = data.last[:sequence_number]
            next_point = get_stream_point brook, {:type=>:after_point, :starting_point=>last_point}
            milliseconds_behind = ((Time.now - data.last[:created_at])*1000).to_i
        else
            last_point = nil
            next_point = point
            milliseconds_behind = 0
        end

        {
            :last_point => last_point,
            :next_point => next_point,
            :milliseconds_behind => milliseconds_behind,
            :records => data
        }

    end

    private
    def get_db name
        raise ArgumentError.new("Stream does not exist.") if !exists?(name) 
        SQLite3::Database.new name
    end

    def get_id_from_rs rs
        return nil if rs == nil
        record = rs.first
        return nil if record == nil
        return nil if record["id"] == nil
        return record["id"].to_i
    end
end