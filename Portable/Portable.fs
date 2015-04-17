namespace Portable

open System.Threading.Tasks;

module Test1 =
    type  AsyncTest() = 
         let myAsync = async {  
            do! Async.Sleep(5000)
            return "aa"  
            }

         member this.StartMyAsyncAsTask cancellation =
                Async.StartAsTask(myAsync, cancellationToken = cancellation)

module Recursive =
    let rec private factorials (num, factorial) = 
       seq { yield (num, factorial)
             let num = num + 1
             yield! factorials(num, num * factorial)
            }
    
    let Factorials = factorials (0, 1)

    let rec private fibonnaci num next = 
       seq { 
             yield num
             yield! fibonnaci next (num + next)
           }
    let Fibonnaci = fibonnaci 0 1

module BigFibonnaci = 
    
    let fib = 
        (0I, 1I) 
            |> Seq.unfold (fun (x, y) -> let z = x + y in Some(z, (y, z))) 
            |> Seq.append [0I;1I]

    let fac = 
        (1I, 2I) 
            |> Seq.unfold (fun (x, y) -> let z = x * y in Some(z, (y, z))) 
            |> Seq.append [1I;2I]