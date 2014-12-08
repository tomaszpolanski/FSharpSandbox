module Seq

    let count projection source =
        source |> Seq.countBy projection
               |> Seq.filter (fun (isTrue, _) -> isTrue)
               |> Seq.take 1
               |> Seq.map (fun (_, count) -> count)
               |> Seq.tryHead 
               |> defaultArg <| 0

    let exists source = 
        source |> Seq.exists (fun _ -> true)