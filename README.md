# Enbilulu
Enbilulu is a lite kinesis-like. It's actually a lib backed by sqlite.

It is intended more as an experiment than anything you, where you is not me, would actually want to use.

![.NET Core](https://github.com/gilmae/enbilulu/workflows/.NET%20Core/badge.svg)

## It's not Kinesis
There is *no* intention of replicating the actual Kinesis API. I have no interest in shards and so on. Kinesis shards solve a problem I don't have. Two problems, actually, if you take into account they are a data partioning mechanism as well as a throughput limiter. 

## What it does do
What I do want is the ability to put records in a stream, and get records from the stream in order, starting from particluar points.

## What it does not yet do
I forsee the following API updates:

* Does stream exist?
* Create stream if not exist.
* Delete record.
* Delete stream, maybe.

## The name
[Enbilulu](https://en.wikipedia.org/wiki/Enbilulu) was a minor god in the Sumerian mythos, the god of rivers and canals. And, you know...streams. 