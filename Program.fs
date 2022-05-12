// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.IO.Pipes

// You've heard of the 2-value boolean
// The 3-value boolean wich has null
// Even perhaps the 4-value boolean
// Now, for a limited time only
// The FIVE value boolean!
// it's the "Foolean"
type Foolean = 
    | Yup       // true
    | Nope      // false
    | Dunno     // null
    | Huh       // IO failure
    | Yeet      // function that returns a Foolean


let pipeoptions = new PipeOptions()
let bar = new PipeStream(pipeoptions)

// Define a function to construct a message to print
let from whom =
    sprintf "from %s" whom


[<EntryPoint>]
let main argv =
    let message = from "F#" // Call the function
    printfn "Hello world %s" message
    0 // return an integer exit code