// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open Api.``Improved scanning impl``
open Api.Domain
open Api.``Improved scanning``

[<EntryPoint>]
let main argv = 
    
    let printFiles (f: ProcessFiles) folder =
        folder |> f |> Seq.iter (printfn "%A")

    printFiles (processFiles) (Folder @"C:\Users\Tomasz\Source\Workspaces\ShareLink") 
    System.Console.ReadLine() |> ignore

    0 // return an integer exit code
