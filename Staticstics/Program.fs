// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open Api.``Improved scanning impl``
open Api.Domain
open Api.``Improved scanning``
open CommonLibrary



[<EntryPoint>]
let main argv = 
    let testArgs = [|@"C:\Users\tomas_000\Documents\Visual Studio 2013\Projects";"*.cs"|]


    let getFolder args =
            match args with
            | [|folder;_|] -> succeed (Folder folder)
            | _ -> fail (ParameterError "No folder on the first place")
            

    let getExtension args =
            match args  with
            | [|_;extension|] -> Api.FileExtension.create extension
            | _ -> fail (ParameterError "No extension on second place")


    let folderOption = getFolder testArgs
    let extensionOption = getExtension testArgs

    let printFiles (f: ProcessFiles) extension folder =
        match  f extension folder with
            | files when Seq.isEmpty files -> printfn "No files found"
            | files -> files |> Seq.iter (printfn "%A")



    let getParams = merge folderOption extensionOption
        
                    
    match getParams with
        | Success (folder, extension) -> printFiles processFiles extension folder
        | Failure e -> printfn "Invalid parameters %A" e
    System.Console.ReadLine() |> ignore
    //C:\Users\Tomasz\Source\Workspaces\ShareLink
    0 // return an integer exit code
