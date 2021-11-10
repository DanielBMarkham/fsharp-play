#!/usr/bin/env -S dotnet fsi

open System
open System.Net
open System.Threading
open System.Threading.Tasks
open Microsoft.FSharp.Control.TaskBuilder

// args if you want 'em. Use idiomatic try catch
let args=System.Environment.GetCommandLineArgs()
let arg1=try Some args.[1] with |_-> None
let arg2=try Some args.[2] with |_-> None
let arg3=try Some args.[3] with |_-> None


let asyncReadIncoming(readSizeNext:int, incomingStreamBuffer:option<Text.StringBuilder>, buffer:ref<byte []>, readSizeCumulative:int, stream:System.IO.Stream)=
  if readSizeNext>(0) // -1 is nothing happened. 0 is end-of-stream
    then      //Try reading again to the end of the stream.
      try
        incomingStreamBuffer.Value.Append(Console.InputEncoding.GetString(!buffer, 0, readSizeNext)) |> ignore 
        let rec asyncReadRest() =
          task {
            let newBuffer=Array.zeroCreate 65535
            do! Task.Yield()
            match stream.Read(newBuffer, 0, newBuffer.Length) with
              | readCount when readCount >0 ->
                  incomingStreamBuffer.Value.Append(Console.InputEncoding.GetString(newBuffer, 0, readCount)) |> ignore
                  return! asyncReadRest()
              |_->return incomingStreamBuffer
          }
        incomingStreamBuffer
      with |ex->
          Console.WriteLine ("INCOMING STREAM. READ FAIL " + ex.Message) |> ignore
          incomingStreamBuffer
  else incomingStreamBuffer


let asyncGetIncomingStream():Task<option<Text.StringBuilder>> =
  task {
    if System.Console.IsInputRedirected then
      use stream = Console.OpenStandardInput()
      let buffer = ref (Array.zeroCreate 65535)
      let readSizeCumulative=ref (-1)
      let incomingStreamBuffer:option<Text.StringBuilder> = Some (new System.Text.StringBuilder())
      try
        let readSizeInitial=
          try
            stream.Read(buffer.Value, 0, (buffer.Value).Length)
          with | :? NotSupportedException as nse->(-1)
        return asyncReadIncoming(readSizeInitial, incomingStreamBuffer, buffer, readSizeInitial, stream)
      with |_->return None
    else return None
  }


printf "hello world\n"
printf "How are you?\n"

task {
  let! incomingStream = asyncGetIncomingStream()
  if incomingStream.IsSome then
    printf "\n You sent me a stream\n"
    printf "%A" incomingStream.Value
    let incomingLines=incomingStream.ToString().Split Environment.NewLine
    let numItems=incomingLines.Length
    printf "You sent me %A items\n" numItems
    //printf "%A init\n" init
    else ()
}

