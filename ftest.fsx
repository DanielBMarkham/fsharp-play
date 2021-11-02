#!/usr/bin/env -S dotnet fsi

open System.Net
open System


let input =
#if INTERACTIVE
// TODO: Enter input here when running in F# Interactive
        "duck"
#else
    System.Console.ReadLine(); 
#endif

printf "hello world\n"


printf "how are you?\n"

Console.WriteLine input
