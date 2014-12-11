#r "../packages/FSharp.Data.2.1.0/lib/net40/FSharp.Data.dll"
#r "System.Xml.Linq.dll"

open FSharp.Data

type data = CsvProvider< "Radio.csv", ";" >
let radioData = data.Load("Radio.csv")

type Language = 
    { folder : string
      faq : string
      legal : string
      privacy : string
      tAndC : string }

let languages = 
    radioData.Rows
    |> Seq.map (fun language -> 
           { Language.folder = language.Folder
             faq = language.FAQ
             legal = language.LEGAL
             privacy = language.Privacy
             tAndC = language.``Terms and Conditions`` })

open System.IO


let createFolder folderName = Directory.CreateDirectory(@"d:\Temp\" + folderName)


type Authors = XmlProvider<"sample.xml">
let sample = Authors.Load("sample.xml")
let processData language = 
    createFolder language.folder |> ignore
    sample.Strings.[0].XElement.Value <- language.faq
    sample.Strings.[1].XElement.Value <- language.legal
    sample.Strings.[2].XElement.Value <- language.privacy
    sample.Strings.[3].XElement.Value <- language.tAndC
    sample.XElement.Save(@"d:\Temp\" + language.folder + "\urlConfig.xml")


languages |> Seq.iter  processData


