#!/usr/bin/env -S dotnet fsi

open System
open System.Net
open System.Threading
open System.Threading.Tasks
open Microsoft.FSharp.Control.TaskBuilder
open System.Collections.Generic
open Microsoft.FSharp.Collections

let flagDict=new System.Collections.Generic.List<string>()
let parmDict=new SortedDictionary<string,string>()
try
  let skipNameOfExecutable=System.Environment.GetCommandLineArgs()|>Array.skip 2
  let splitArgIntoNameAndValue (s:string)=if s.Contains ":" then s.Split(":",2) else s.Split("=",2)
  let isAParm (s:string) = if s.Contains ":" || s.Contains "=" then true else false
  let cParms,cFlags = skipNameOfExecutable|>Array.partition isAParm
  cFlags |> Array.iter(fun x->flagDict.Add(x))
  cParms |> Array.iter(fun x->parmDict.Add((splitArgIntoNameAndValue x).[0],(splitArgIntoNameAndValue x).[1]))
with |ex->()

let asyncReadIncoming(readSizeNext:int, incomingStreamBuffer:option<Text.StringBuilder>, buffer:ref<byte []>, readSizeCumulative:int, stream:System.IO.Stream)=
  if readSizeNext>(0) // -1 is nothing happened. 0 is end-of-stream
    then      //Try reading again to the end of the stream.
      try
        incomingStreamBuffer.Value.Append(Console.InputEncoding.GetString(buffer.Value, 0, readSizeNext)) |> ignore
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
printf "=============\n"
printfn "FLAGS"
flagDict |> Seq.iter(fun x->printfn "%A" x)
printfn "PARMS"
parmDict |> Seq.iter(fun x->printfn "%A" x)

task {
  let! incomingStream = asyncGetIncomingStream()
  if incomingStream.IsSome then
    printf "\nIncoming stream detected\n"
    let incomingLines=incomingStream.Value.ToString().Split Environment.NewLine |> Array.toList |>List.filter(fun x->x<>"")
    // DUMMY JOB FOR THIS SCRIPT TO DO
    let sortedIncoming =
      if flagDict.Exists(fun x->x="asc")
        then
          incomingLines |> List.sort
        else incomingLines |> List.sort |>List.rev

    let numItems=sortedIncoming.Length
    printf "\nYou sent me %A items\n" numItems
    sortedIncoming|>List.iter(fun x->printfn "%s" x) |> ignore
    else ()
}

