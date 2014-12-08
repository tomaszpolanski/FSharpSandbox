// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System
open System.Threading

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    let myAsync =
        async {
            while true do
                do! Async.Sleep(1000)
                printf "%A \n" DateTime.Now 
        }
    let tokenSource1 = new System.Threading.CancellationTokenSource(TimeSpan.FromMilliseconds 5000.)
    let val1 = Async.Start(myAsync, cancellationToken=tokenSource1.Token) 

    Console.ReadLine() |> ignore
    0 // return an integer exit code
