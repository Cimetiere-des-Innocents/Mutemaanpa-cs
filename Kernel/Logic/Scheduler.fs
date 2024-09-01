namespace Kernel

open GameSys

(*
Scheduler:
    In this module I implement a scheduler for orchestrating all systems.
    
    The basic functionality of a scheduler is to run a series of systems. When the session updates,
    it use its scheduler to run all systems. 
*)

(* Scheduler runs in the world, finding resources to execute system functions. *)
type Scheduler(gameSys: (World -> unit) list) =
    let mutable gameSys = gameSys

    member _.run world =
        gameSys |> List.map (fun sys -> sys world) |> ignore

    member _.add(sys: World -> unit) = gameSys <- sys :: gameSys

    member _.add(sys: Res<'a> -> unit) =
        gameSys <- (makeSystemRes sys) :: gameSys


module Scheduler =
    let newScheduler () = Scheduler(List.empty)
