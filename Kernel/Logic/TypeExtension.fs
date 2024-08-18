namespace Kernel

(*
TypeExtension:
    Utilities to enhance C# oriented libraries, making them more like F#.
*)
module Extension =
    type System.Collections.Generic.IDictionary<'Key, 'Value> with
        (* return values by parameters is way too imperative, so this use option to wrap it. *)
        member this.TryGet key =
            let ok, v = this.TryGetValue key
            if ok then Some v else None

    type System.Collections.Generic.Dictionary<'Key, 'Value> with
        member this.ToArray() =
            [| for x in this do
                   yield (x.Key, x.Value) |]
