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

###hyperparams
thresh = .8
n = 33
ps = 100
mutR = 0.8
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

mpcnn = MPCNN()

def fitK(x):
    Fnorm = nn.functional.normalize(x)
    Fdif = [nn.functional.pairwise_distance(Fnorm[i],Fnorm[i+1]) for i in range(len(Fnorm))]
    D = torch.abs(Fdif)
    return torch.min(D) + torch.mean(D)




##evolve SRN controller in parallel
class SRN(nn.Module):
    def __init__(self):
        super().__init__()
        self.state = None
        self.hiddenrec = nn.Linear(6, 3, bias=True) #tanh activation built-in *paper uses sigmoid*
        self.outlayer = nn.Linear(3, 3, bias=True)
    def forward(self, x): #input: tensor of shape (L,N,Hin)(L, N, H_{in})(L,N,Hin​)
        if self.state is None:
            self.state = torch.zeros_like(x)
        self.state = self.hiddenrec(torch.cat(self.state,x)) #output: tensor of shape (L,N,D∗Hout)(L, N, D * H_{out})(L,N,D∗Hout​)
        out = self.outlayer(self.state)
        return out

srn = SRN()



#steering = (x[0] + x[1])/2
#throttle = x[2]


##CoSyNE encoding
def make_C(xt): #xt=control signal for num simulation control steps
    return (1/(3*xt.shape(1)))*(torch.sum(torch.sum(torch.pairwise_distance(xt[i for i in range(len(xt))], xt[i for i in range(len(xt))]).square())))


def fitCOS(p,xt): #d=distance along track axis from start line; v=max speed; m=cumulativedamage
    d,m,v = run_mpcnn_rnn(xt)
    return d - ((3*m)/1000) + (v/5) - (100*make_C(xt))

def reproduction(s_p, swap):
    n_parents, n_genes = s_p.shape
    # get all combinations of parents
    combinations = [[p1, p2] for p1 in range(n_parents-1, 0, -1) for p2 in range(p1-1, -1, -1)]
    # select some of these combinations randomly for reproduction
    comb_sample = list(np.random.choice(len(combinations), size=(n_parents), replace=False))
    combinations = [comb for i, comb in enumerate(combinations) if i in comb_sample]
    # initialize an array to fill the children with
    children = np.zeros(s_p.shape)
    for i_comb, comb in enumerate(combinations):
        parent1, parent2 = s_p[comb]
        child = parent1.copy()
        # mask each parent's genes randomly with an equal probability
        child[swap] = parent2[swap]
        children[i_comb, :] = child
        return children

def recomb(p, eval, mutR):
    p_top = torch.sort(torch.cat((eval,p),1))[0][:int(ps*.2)]
    inds = torch.multinomial(p_top, int(len(p_top)*mutR))
    #crossover
    children = reproduction(p_top,inds)
    #mutate
    children[inds] = torch.rand(len(inds))
    return children


def CoSyNE(p, thresh, fitness_FX): #p = population x weights (nxm)=33x100
    for g in range(generations):
        evals = []
        for j in range(ps):
            eval = fitness_FX(p[:,j]) #form complete solution and evaluate with fitness
            evals.append(eval)
        if evals.any() >= thresh:
            break
        o = recomb(p, evals, mutR)
        for i in range(n):
            p = torch.sort(torch.cat((eval,p),1))[0]
            p[i] = torch.permute(p[i], torch.randperm(n))
            for k in range(int(len(p)*mutR)):
                p[k] = o[k] #replace least fit weights with weights from offspring nets
    return w, eval



####run
##evolve mpcnn seperately according to fit_K

data_sample = torch.rand(24,1,64,64)
feature_out = mpcnn(data_sample)

f = torch.cat([a.flatten() for a in mpcnn.parameters()])


w, eval = CoSyNE(mpcnn.parameters(), thresh, fitK)

##evolve mpcnn:srn together according fitcos





####Vestigule codes

#     for i, data in enumerate(trainloader, 0):
#         inputs, labels = data
#         outputs = net(inputs)
#
# PATH = './cifar_net.pth'
# torch.save(net.state_dict(), PATH)
#
#
# dataiter = iter(testloader)
# images, labels = dataiter.next()
#
# # print images
# imshow(torchvision.utils.make_grid(images))
# print('GroundTruth: ', ' '.join('%5s' % classes[labels[j]] for j in range(4)))
































#
