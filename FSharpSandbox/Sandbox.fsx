#r "../packages/FSharp.Data.2.1.0/lib/net40/FSharp.Data.dll"
#r "System.Xml.Linq.dll"

open FSharp.Data
open System.IO

type data = CsvProvider< "Radio.csv", ";" >
let radioData = data.Load("Radio.csv")

type Language = 
    { folder : string
      faq : string
      legal : string
      privacy : string
      tAndC : string }

let languages = 
    radioData.Rows |> Seq.map (fun language -> 
                          { Language.folder = language.Folder
                            faq = language.FAQ
                            legal = language.LEGAL
                            privacy = language.Privacy
                            tAndC = language.``Terms and Conditions`` })

let createFolder folderName = Directory.CreateDirectory(@"d:\Temp\" + folderName)

type Authors = XmlProvider< "sample.xml" >
let sample = Authors.Load("sample.xml")

let processData language = 
    createFolder language.folder |> ignore
    sample.Strings.[0].XElement.Value <- language.faq
    sample.Strings.[1].XElement.Value <- language.legal
    sample.Strings.[2].XElement.Value <- language.privacy
    sample.Strings.[3].XElement.Value <- language.tAndC
    sample.XElement.Save(@"d:\Temp\" + language.folder + "\urlConfig.xml")

languages |> Seq.iter processData

module ``File scanning`` =
    
    open System.IO;

    let blackList = [".g.cs"; "g.i.cs"; "AssemblyInfo.cs"]

    let filterFile blList (file:string) = 
        blList |> List.forall (file.EndsWith >> (not))

    let filter = filterFile blackList

    let rec readDir dirName = 
        seq {
            let files = Directory.GetFiles(dirName, "*.cs")  |> Array.filter filter
            

            let dirs = Directory.GetDirectories(dirName) 
            yield! dirs |> Seq.collect readDir 
            yield files
        }

    let test = readDir @"C:\Users\Tomasz\Source\Workspaces\ShareLink"

    test |> Seq.collect  Array.toSeq 
         |> Seq.map (fun name -> async { return name, File.ReadAllLines(name).Length })
         |> Async.Parallel
         |> Async.RunSynchronously
         |> Array.filter (fun (_, lines) -> lines <> 0)
         |> Array.map (fun (name, lines) -> Path.GetFileName(name), lines )
         |> Array.sortBy (fun (_, lines) -> -lines)