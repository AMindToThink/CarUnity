#https://dl.acm.org/doi/pdf/10.1145/2576768.2598358
# https://www.jmlr.org/papers/volume9/gomez08a/gomez08a.pdf

import torch
import torchvision
import torchvision.transforms as transforms
import torch.nn.functional as F
import torch.optim as optim
import torch.nn as nn

import matplotlib.pyplot as plt
import numpy as np

import mlagents
from mlagents_envs.environment import UnityEnvironment as UE
# import colorsys

###hyperparams
thresh = 5
N = 583
ps = 100
mutR = 0.8
gens = 3
k = 100
#


##evolve MPCNN
class MPCNN(nn.Module):
    def __init__(self):
        super().__init__()
        self.conv1 = nn.Conv2d(1, 10, 2)
        self.pool = nn.MaxPool2d(3, 3)
        self.conv2 = nn.Conv2d(10, 10, 2)
        self.pool2 = nn.MaxPool2d(2, 2)
        self.conv3 = nn.Conv2d(10, 3, 2)
        self.pool3 = nn.MaxPool2d(2)
        self.f  = nn.ReLU()
    def forward(self, x):
        x = self.pool(self.conv1(x))
        x = self.pool2(self.f(self.conv2(x)))
        x = self.pool2(self.f(self.conv2(x)))
        x = self.pool3(self.f(self.conv3(x)))
        return x


def rgb2hsv(im):
    return np.asarray(im.fromarray().convert('HSV'))



env = UE(file_name='/home/whale/CarUnity/RachelCar/car.x86_64', seed=1, side_channels=[])

env.reset()
behavior_name = list(env.behavior_specs)[0]
spec = env.behavior_specs[behavior_name]
data_sample = torch.zeros(k, 64, 64, 1)
for episode in range(3):
  env.reset()
  decision_steps, terminal_steps = env.get_steps(behavior_name)
  tracked_agent = -1
  done = False
  while not done:
      i = 0
    if tracked_agent == -1 and len(decision_steps) >= 1:
      tracked_agent = decision_steps.agent_id[0]
    ###----------------------This code needs to be fixed------------------
    action = spec.random_action(len(decision_steps)) #what is the new version of this command????!?!?!?!!?!?
    env.set_actions(behavior_name, action) #should be fine
    env.step() #should be fine
    decision_steps, terminal_steps = env.get_steps(behavior_name)
    obs = rgb2hsv(np.asarray(decision_steps.obs)) ### check this, probably a shape issue
    data_sample[i,...] = obs[:,:,2] #depends on shapes above
    ###-----------------------end fix code------------------------------------
data_sample = data_sample.permute(0,3,1,2)
# data_sample = torch.ones(100,1,64,64)
p = torch.zeros(N,ps,ps)

for g in range(gens):
    for i in range(p.shape[1]):
        mpcnn = MPCNN()
        if g == 0:
            p[:,i,0] = torch.cat([a.flatten() for a in mpcnn.parameters()])
        elif g > 0:
            wi=0
            for param in mpcnn.parameters():
                param = nn.parameter.Parameter(p[wi,i,0])
                wi+=1
        feature_out = mpcnn(data_sample)
        Fnorm = nn.functional.normalize(feature_out)
        Fnorm_shift = torch.roll(Fnorm, shifts=1)
        Fdif = nn.functional.pairwise_distance(Fnorm, Fnorm_shift).abs()
        p[:,:,i] = torch.min(Fdif) + torch.mean(Fdif)
    evals = p[0,0,:] #bigger=better, "maximize"
    print('gen:', g, 'max fitness:', torch.max(evals))
    p_sort = torch.sort(p, 2)[0]
    p_top = p_sort[:,:int(ps*.2),:int(ps*.2)]
    inds = torch.multinomial(p_top[0,0,:], int(p_top.shape[1]*mutR))
    n_genes, n_parents, _ = p_top.shape
    combinations = [[p1, p2] for p1 in range(n_parents-1, 0, -1) for p2 in range(p1-1, -1, -1)]
    comb_sample = list(np.random.choice(len(combinations), size=(n_parents), replace=False))
    combinations = [comb for i, comb in enumerate(combinations) if i in comb_sample]
    children = torch.zeros(p_top.shape)
    for i_comb, comb in enumerate(combinations):
        parent1 = p_top[:, comb[0],0]
        parent2 = p_top[:, comb[1],0]
        child = parent1.clone()
        child[inds] = parent2[inds]
        children[:, i_comb, 0] = child
    children[:,inds,0] = torch.rand(len(inds))
    for i in range(p.shape[1]):
        p[:,i,0] = p[torch.randperm(len(p)), i, 0]
    p[:,-int(ps*.2):,-int(ps*.2):] = children





