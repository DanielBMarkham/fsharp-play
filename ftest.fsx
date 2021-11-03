#!/usr/bin/env -S dotnet fsi

open System.Net
open System


// args if you want 'em. Use idiomatic try catch
let args=System.Environment.GetCommandLineArgs()
let arg1=try Some args.[1] with |_-> None
let arg2=try Some args.[2] with |_-> None
let arg3=try Some args.[3] with |_-> None

// Get any (single) incoming file stream from the console
// DOES NOT HANDLE MULTIPLE OR NAMED STREAMS
let getIncomingStream() =
  let stream = Console.OpenStandardInput()
  let buffer = ref (Array.zeroCreate 65535)
  let readSize=ref (-1)
  let incomingStreamBuffer = new System.Text.StringBuilder()
  let readSizeInitial=
    try
      stream.Read(!buffer, 0, (!buffer).Length)
      with | :? NotSupportedException as nse->(-1)
  let readIncoming()=
    if readSizeInitial>(0) // -1 is nothing happened. 0 is end-of-stream
      then
        incomingStreamBuffer.Append(Console.InputEncoding.GetString(!buffer, 0, readSizeInitial)) |> ignore 
        readSize:=readSizeInitial
        //Try reading again to the end of the stream.
        try
          let newBuffer=Array.zeroCreate 65535
          let rec readRest() =
            match stream.Read(newBuffer, 0, newBuffer.Length) with
              | readCount when readCount >0 ->
                incomingStreamBuffer.Append(Console.InputEncoding.GetString(newBuffer, 0, readCount)) |> ignore 
                readSize:=!readSize+readCount
                readRest()
              |_ ->()
          readRest()
          with |ex->
            Console.WriteLine ("INCOMING STREAM. SECOND READ FAIL " + ex.Message)
      else ()
  readIncoming()
  incomingStreamBuffer


printf "hello world\n"


printf "How are you?\n"

printf "\n"
let incoming=getIncomingStream()
printf "%A" incoming
let numItems=incoming.Length
printf "You sent me %A items\n" numItems

