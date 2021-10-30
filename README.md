# MOBS

## How to add a new crowd

- From Unity toolbar, select MOBS > Crowd
- Select agent roles in this group.
- Input the number of agents in the group. 
- Select the width and length of the rectangular area where you want your group placed.
- Click on Add Group.


In your scene, you will see the new group as a child of the Crowd game object.

## How to add a new character model

- When adding a new character model, first move the fbx file to Assets > Character Models. All the human models in fbx format, their textures and materials are under this folder.  
- Click on the fbx in Unity.
- In the inspector, select the "Rig" tab and make the "Animation Type" "Humanoid". Avatar Definition should be "Create From This Model".
- Select "Materials" tab, "Extract Textures" into Character Models > Textures folder. "Extract Materials" into Character Models > Materials folder.
- Adjust the model ise from the "Model" tab under "Scale Factor" if necessary.
- Move the fbx model into the scene. Click on the model in the Hierarchy.
- Animator Controller must be "Default Controller"
- Add NavMeshAgent
- Add Sphere Collider. Check "Is Trigger" box. It will be kinematic trigger.
- Add Capsule Collider. Adjust its size to fit the model.
- Add RigidBody. Set its drag and angular drag to a high value such as 100 so that characters don't get pushed around too much.
- Move the character to the "Resources" folder to make it a prefab.
- In the code, "GroupBuilder.cs",  add the character's name to the list of charNames. 






