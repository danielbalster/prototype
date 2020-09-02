# simple behaviortree #

## blackboard 

- has a (key,value) pair per node scope. use the empty guid for tree scope

## specific state stored in blackboard

 ?!#$ are internal keys, they can be hidden in editors.
- ? = is a node currently open or closed?
- ! = last state of a node
- # = iterator of sequences/selectors

## selector and sequence are implementing the "memory" pattern, they do only one iteration per loop
