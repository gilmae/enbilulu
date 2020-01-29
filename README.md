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

Having said that, this has now been through two iterations. it started as just libraries (ruby and pNp) for accessing a sqlite db. now its a dotnet NancyFx web api for accessing a sqlite db. That’s a regression really; at the very least it should *also* be a library for directly accessing the sqlite db.

After all, sqlite calls itself a serverless db in the sense that it does not need a sepwrate server and the client-server pattern to use. It can be used in the same process as the user program. I’ve taken a step away from that for thr purposes of a yak shave, but...

On the other hand, I’ve had from the beginning an idea of trying to create this as a db itself. I don’t know what that looka like, but there it is.

## The name
[Enbilulu](https://en.wikipedia.org/wiki/Enbilulu) was a minor god in the Sumerian mythos, the god of rivers and canals. And, you know...streams. 