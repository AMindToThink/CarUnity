
import mlagents
import colorsys
from mlagents_envs.environment import UnityEnvironment as UE

#def RGBToHSL(r, g, b):
#  r /= 255.0
#  g /= 255.0
#  b /= 255.0
#  min = min(r, min(g, b))
#  max = max(r, max(g, b))
#  lumin = (max + min)/2
#  sat = 0
#  if lumin <= .5:
#    sat = (max-min)/(max+min)
#  else:
#    sat = (max-min)/(2.0-max-min)
#  hue = 0
#  if r == max:
#    hue = (g-b)/(max-min)
#  elif g == max:
#    hue = 2.0 + (b-r)/(max-min)
#  else:
#    hue = 4.0 + (r-g)/(max-min)
#  hue *= 60
#  if hue < 0:
#    hue += 360.0
#  return [round(hue, 0), round(sat, 2), round(value, 2)]
env = UE(file_name='RollerballBuild', seed=1, side_channels=[])

env.reset()

behavior_name = list(env.behavior_specs)[0]
print(f"Name of the behavior : {behavior_name}")
spec = env.behavior_specs[behavior_name]


print("Number of observations : ", len(spec.observation_shapes))

if spec.is_action_continuous():
  print("The action is continuous")

if spec.is_action_discrete():
  print("The action is discrete")

decision_steps, terminal_steps = env.get_steps(behavior_name)
print(decision_steps.obs)


for episode in range(3):
  env.reset()
  decision_steps, terminal_steps = env.get_steps(behavior_name)
  
  tracked_agent = -1 # -1 indicates not yet tracking
  done = False # For the tracked_agent
  episode_rewards = 0 # For the tracked_agent
  while not done:
    # Track the first agent we see if not tracking
    # Note : len(decision_steps) = [number of agents that requested a decision]
    if tracked_agent == -1 and len(decision_steps) >= 1:
      tracked_agent = decision_steps.agent_id[0]
    # Generate an action for all agents
    action = spec.create_random_action(len(decision_steps))
    # Set the actions
    env.set_actions(behavior_name, action)
    # Move the simulation forward
    env.step()
    # Get the new simulation results
    decision_steps, terminal_steps = env.get_steps(behavior_name)
    if tracked_agent in decision_steps: # The agent requested a decision
      episode_rewards += decision_steps[tracked_agent].reward
    if tracked_agent in terminal_steps: # The agent terminated its episode
      episode_rewards += terminal_steps[tracked_agent].reward
      done = True
  print(f"Total rewards for episode {episode} is {episode_rewards}")



env.close()
print("Closed environment")
