(* TypeExtension *)
namespace Kernel

module Extension =
    type System.Collections.Generic.IDictionary<'Key, 'Value> with
        member this.TryGet key =
            let ok, v = this.TryGetValue key
            if ok then Some v else None

    type System.Collections.Generic.Dictionary<'Key, 'Value> with
        member this.ToArray() =
            [| for x in this do
                   yield (x.Key, x.Value) |]
