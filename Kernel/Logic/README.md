# Game logic

The pure logic part of Mutemaanpa.

It does not care where the data come from, where input come from, etc. It just process message on
its components through some system.

## Component and system registry

Component is some resources. They are manipulated by some systems when some events came.

So we need to register some handler responding to some event, who then forward the message to
system to process.
