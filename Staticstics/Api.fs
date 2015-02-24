module Api
open CommonLibrary

module Opt = 
    
    let tryGet f = 
        try Some (f()) with e -> None



module Domain =
    
    type FileUri = FileUri of string
    type Folder = Folder of string

    type Errors =
        | InvalidExtension of string
        | InvalidFolder of string
        | InvalidFile of FileUri
        | ParameterError of string
        | NoLines


module FileName =
    
    open Domain
    open System.IO

    type T = FileName of string

    let create (fileUri: FileUri) =
       let (FileUri value) = fileUri
       let fileNameOption = Opt.tryGet (fun () -> Path.GetFileName( value)) |> Option.map FileName
       match fileNameOption with
        | Some file -> succeed file
        | None -> fail (InvalidFile fileUri)

    let apply f (FileName e) = f e

    let value = apply id 

module FileExtension =
    open System
    open Domain

    type T = FileExtension of string

    let create (str: String) = 
        match str.StartsWith "*." with
            | true -> succeed (FileExtension str)
            | false -> fail (InvalidExtension str)

    let apply f (FileExtension e) = f e

    let value = apply id 
        
module FileInformation =
    
    open Domain

    type T = {fileName:FileName.T; lineCount: int}

    let create  count name =
       match count with
        | c when c > 0 ->
            Success {fileName = name; lineCount = c}
        | _ ->
            Failure NoLines

module ``Improved scanning`` = 

    open Domain
    open FileName
   

    type FileInformation = {fileName: FileName.T; lineCount: int}

    type ScanFolder = FileExtension.T -> Folder ->  FileUri seq

    type ProcessFiles = FileExtension.T -> Folder ->  Result<FileInformation.T,Errors> seq

module ``Improved scanning impl`` = 
    
    open ``Improved scanning``
    open Domain
    open System.IO
    open FileExtension

    let scanFolder extension dir  = 
        let rec readDirReq folder = seq {
            let (Folder currentFolder) = folder
            let files = Opt.tryGet (fun () ->  Directory.GetFiles(currentFolder, FileExtension.value extension))
                         |> Option.map (Seq.map FileUri)
            

            let dirs = Opt.tryGet (fun () -> Directory.GetDirectories(currentFolder))
                        |> Option.map (Seq.map Folder)
            yield! (match dirs with | Some s -> s | None -> Seq.empty) |> Seq.collect readDirReq 
            yield match files with | Some s -> s | None -> Seq.empty
        } 

        readDirReq dir |> Seq.collect id

    let processFiles extension dir  = 
        let createInfo (name,length) =
             name |> FileName.create 
                  |> CommonLibrary.bind (FileInformation.create length)

        let hasNoLines = function
            | Failure (NoLines) -> true
            | _ -> false

        dir |> scanFolder extension 
            |> Seq.map (fun name -> async { return name, File.ReadAllLines(match name with FileUri s -> s).Length })
            |> Async.Parallel
            |> Async.RunSynchronously
            |> Seq.map createInfo
            |> Seq.filter (hasNoLines >> not)

    
