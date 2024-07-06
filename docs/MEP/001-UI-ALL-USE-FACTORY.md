# MEP 001

Currently our ui are using both Godot Editor & Code to create and assemble Control nodes.

The editor is quite convenient because it helps us arrange the content interactively, but
it is unfortunately too inflexible for manipulation of stateful data.

Also, it is not very ideal for code quality because it forbids us to assert some null-relating contracts like one often achieved using ctors.

So in longer term it must be replaced, at least in gui modules.
