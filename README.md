### multi-selection prototype ###

## purpose

This provides a small framework to prototype ideas, especially regardings multi-selection in RTS games.

## todo

[_] cylindrical mesh for ws selection with half-transparent material
[_] mouse click into world hittest (floor, object)
  - for debugging only. this is still a PC app
[_] input behavior tree?
  - conditions
      get button
	  get axis
	  get selection empty?
	  get selection count
  - actions
    - set variable
	- increase variable
	- set behavior to selected units
	- 

select group
- hold A to add units
- hold B to remove units

give command
- press Y to choose command

move controller
- target under crosshair
- press A to confirm target
- press B to cancel

BehaviortreeViewModel has a root NodeViewModel
- have a template selector
- fill an items control
- sequence is vertical
- selector is horizontal
- decorator sits on top of a child

decorator (add remove clear)
child

selector (add remove clear)
A B C

sequence
A
B
C

child (add before, add after, add parent)