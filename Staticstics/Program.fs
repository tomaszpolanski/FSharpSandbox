// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

module ``File scanning`` =
    
    open System.IO;

    let blackList = ["R.java"]

    let filterFile blList (file:string) = 
        blList |> List.forall (file.EndsWith >> (not))

    let filter = filterFile blackList

    let rec readDir dirName = 
        seq {
            let files = Directory.GetFiles(dirName, "*.java")  |> Array.filter filter
            

            let dirs = Directory.GetDirectories(dirName) 
            yield! dirs |> Seq.collect readDir 
            yield files
        }

    let scan dir = 
        readDir dir
         |> Seq.collect  Array.toSeq 
         |> Seq.map (fun name -> async { return name, File.ReadAllLines(name).Length })
         |> Async.Parallel
         |> Async.RunSynchronously
         |> Array.filter (fun (_, lines) -> lines <> 0)
         |> Array.map (fun (name, lines) -> Path.GetFileName(name), lines )
         |> Array.sortBy (fun (_, lines) -> -lines)



    

[<EntryPoint>]
let main argv = 

    let printTotalLines folder =
        folder |> Seq.map (fun (_, lines) -> lines)
               |> Seq.fold (+) 0
               |> printfn "Total lines: %A" 

    let printFiles folder = 
        folder |> Seq.truncate 150
               |> Seq.iter (fun (name, lines) -> printfn "%s: %d lines" name lines) 

    let rec display dir = 
        let folder = match dir with
                            | Some s -> s |> ``File scanning``.scan |> Array.toSeq
                            | None -> Seq.empty
                            
        printTotalLines folder
        printFiles folder

        printfn ""
        printf "Enter path to scan: " 
        System.Console.ReadLine() 
            |> Some
            |> display


    argv |> Array.tryFind (fun _ -> true)
         |> display

    0 // return an integer exit code
