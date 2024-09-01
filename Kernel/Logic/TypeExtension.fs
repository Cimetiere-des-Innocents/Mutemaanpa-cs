namespace Kernel

(*
TypeExtension:
    Utilities to enhance C# oriented libraries, making them more like F#.
*)
module Extension =
    type System.Collections.Generic.IDictionary<'Key, 'Value> with
        (* return values by parameters is way too imperative, so this use option to wrap it. *)
        member self.TryGet key =
            let ok, v = self.TryGetValue key
            if ok then Some v else None

    type System.Collections.Generic.Dictionary<'Key, 'Value> with
        member self.ToArray() =
            [| for x in self do
                   yield (x.Key, x.Value) |]
        member self.AllPrint() =
            self.ToArray()
            |> Array.map (fun (k, v) -> $"key: {k}, value: {v}")
            |> seq
            |> String.concat "\n"

    type System.Collections.Generic.Queue<'Item> with
        member self.FDequeue() =
            let ok, v = self.TryDequeue()
            if ok then Some v else None
