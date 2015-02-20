module Api

module Opt = 
    
    let tryGet f = 
        try Some (f()) with e -> None

module Domain =
    
    type FileUri = FileUri of string
    type Folder = Folder of string

module FileName =
    
    open Domain
    open System.IO

    type T = FileName of string

    let create (fileUri: FileUri) =
       let (FileUri value) = fileUri
       Opt.tryGet (fun () -> Path.GetFileName( value))
                        |> Option.map FileName
        
module FileInformation =
    
    open Domain

    type T = {fileName: FileName.T; lineCount: int}

    let create  count name =
       match count with
        | c when c > 0 ->
            Some {fileName = name; lineCount = c}
        | _ ->
            None

module ``Improved scanning`` = 

    open Domain
    open FileName
   

    type FileInformation = {fileName: FileName.T; lineCount: int}

    type ScanFolder = Folder -> FileUri seq

    type ProcessFiles = Folder -> FileInformation.T seq

module ``Improved scanning impl`` = 
    
    open ``Improved scanning``
    open Domain
    open System.IO

    let scanFolder dir = 
        let rec readDirReq folder = seq {
            let (Folder currentFolder) = folder
            let files = Opt.tryGet (fun () ->  Directory.GetFiles(currentFolder, "*.cs"))
                         |> Option.map (Seq.map FileUri)

                         
            

            let dirs = Opt.tryGet (fun () -> Directory.GetDirectories(currentFolder))
                        |> Option.map (Seq.map Folder)
            yield! (match dirs with | Some s -> s | None -> Seq.empty) |> Seq.collect readDirReq 
            yield match files with | Some s -> s | None -> Seq.empty
        } 

        readDirReq dir |> Seq.collect id

    let processFiles dir = 
        let createInfo (name,length) =
             name |> FileName.create 
                  |> Option.map (FileInformation.create length)
                  |> Option.bind id

        dir |> scanFolder 
            |> Seq.map (fun name -> async { return name, File.ReadAllLines(match name with FileUri s -> s).Length })
            |> Async.Parallel
            |> Async.RunSynchronously
            |> Seq.map createInfo
            |> Seq.choose id

    
