namespace Luncher.Api

module FileSystem =
    open System
    open System.IO

    let parseLine (line : String) =
        line.Split([|','|])


