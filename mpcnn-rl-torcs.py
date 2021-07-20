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

#
batch_size = 4
inp_size = 64

#

# transform = transforms.Compose([transforms.ToTensor(), transforms.Normalize((0.5, 0.5, 0.5), (0.5, 0.5, 0.5))])
#
# trainset = torchvision.datasets.CIFAR10(root='./data', train=True,download=True, transform=transform)
# trainloader = torch.utils.data.DataLoader(trainset, batch_size=batch_size, shuffle=True, num_workers=2)
#
# testset = torchvision.datasets.CIFAR10(root='./data', train=False, download=True, transform=transform)
# testloader = torch.utils.data.DataLoader(testset, batch_size=batch_size, shuffle=False, num_workers=2)
#
# classes = ('plane', 'car', 'bird', 'cat', 'deer', 'dog', 'frog', 'horse', 'ship', 'truck')
#

# def imshow(img):
#     img = img / 2 + 0.5     # unnormalize
#     npimg = img.numpy()
#     plt.imshow(np.transpose(npimg, (1, 2, 0)))
#     plt.show()

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
        # self.fc1 = nn.Linear(16 * 5 * 5, 120)
        # self.fc2 = nn.Linear(120, 84)
        # self.fc3 = nn.Linear(84, 10)

    def forward(self, x):
        x = self.pool(self.conv1(x))
        x = self.pool2(self.f(self.conv2(x)))
        x = self.pool2(self.f(self.conv2(x)))
        x = self.pool3(self.f(self.conv3(x)))
        return x


mpcnn = MPCNN()




F = net(inputs)

Fnorm = nn.functional.normalize(F)

Fdif = nn.functional.pairwise_distance(F[i for i in range(len(F))], F[j for j in range(len(F)-1)])

D = torch.abs([Fdif])

fit_K = torch.min(D) + torch.mean(D)


##evolve SRN controller in parallel
class SRN(nn.Module):
    def__init__(self):
        super().__init__()
        self.hidden = nn.RNN(3, 3, 1) #tanh activation built-in *paper uses sigmoid*
    def forward(self, x): #input: tensor of shape (L,N,Hin)(L, N, H_{in})(L,N,Hin​)
        x = self.hidden(x) #output: tensor of shape (L,N,D∗Hout)(L, N, D * H_{out})(L,N,D∗Hout​)
        return x

srn = SRN()



#steering = (x[0] + x[1])/2
#throttle = x[2]




##CoSyNE encoding
thresh =
n = 33
ps = 100
mutR = 0.8

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


""" where f (xi j ) is the fitness of subgenotype (weight) xi j , and f minj and f maxj are, respectively, the fitness
of the least and most fit individuals in subpopulation i. In this case, the probability of disrupting the
network x j is inversely proportional to its relative fitness, so that weight combinations that receive
high fitness are more likely to be preserved, while those with low fitness are more likely to be
disrupted and their constituents used to search for new complete solutions. In the experiments below,
the simpler function that permutes all weights, except for the newly inserted offspring weights, was
found to work well. """

def CoSyNE(p, thresh): #p = population x weights (nxm)=33x100
    for g in range(generations):
        evals = []
        for j in range(ps):
            eval = fitCos(p[:,j]) #form complete solution and evaluate with fitness
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
##evolve mpcnn:srn together according fitcos





####Vestigule codes
# criterion = nn.CrossEntropyLoss()
# optimizer = optim.SGD(net.parameters(), lr=0.001, momentum=0.9)
#
#
# for epoch in range(2):  # loop over the dataset multiple times
#
#     running_loss = 0.0
#     for i, data in enumerate(trainloader, 0):
#         # get the inputs; data is a list of [inputs, labels]
#         inputs, labels = data
#
#         # zero the parameter gradients
#         optimizer.zero_grad()
#
#         # forward + backward + optimize
#         outputs = net(inputs)
#         loss = criterion(outputs, labels)
#         loss.backward()
#         optimizer.step()
#
#         # print statistics
#         running_loss += loss.item()
#         if i % 2000 == 1999:    # print every 2000 mini-batches
#             print('[%d, %5d] loss: %.3f' %
#                   (epoch + 1, i + 1, running_loss / 2000))
#             running_loss = 0.0
#
# print('Finished Training')
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
