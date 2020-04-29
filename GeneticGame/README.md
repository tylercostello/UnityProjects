This is a game that demonstrates a neuroevolution algorithm. 100 players are instantiated with random parameters in their neural networks. They die if they touch a square. Their neural networks parameters are stored as their DNA. The top two from the previous generation reproduce by instantiating new players with the crossed over and mutated DNA of their parents. If they are the champion of their generation they are reincarnated each generation from then on. Thus the first gen has 100 players, the second has 101, and so on, as the champions are retained. 


This is by no means a perfect example of a genetic algorithm, and the one used is fairly basic as far as genetic algorithms go. This is not a NEAT type algorithm where the structure of the neural network is evolved along with the parameters. These contain a set architecture of 5 input nodes, 3 nodes in the hidden layer, and 1 node in the output layer. The 5 inputs are the distances of 5 raycasts extending from the player, and the output is either a 0 or a 1 depending on the input.


Some improvements would include and randomized parent selection based on fitness, not just the top two. Another improvement would be to give the player more information about its environment through increased raycasts. 

The biggest problems with this algorithm is that it does not retain talent well, and that unity is an inherently clunky, high-usage, and limiting piece of software. Although it may be an inherent problem of using a genetic algorithm, but I haven't researched them enough to be sure. However this was more or less to be expected. 

The first problem is mostly likely caused by the fact that the falling cubes placements are random and therefore some players will simply get lucky and appear to have high performance when in fact it was specific to their situation.

The second problem is expected because this is not unity's intended purpose. When using a user friendly software for making games to simulate hundreds of neural networks, problems are expected, and that is the price one pays for its ease of use.

However, I find that some of the problems encountered are inexcusable. The biggest one being its failure to handle the List class well. 
While it is entirely possible that this is my fault and some oversight in the execution or of code, I think it is more likely that the problem was a result of unity's system of passing execution time between scripts.

The problem in question appears in the Population2(I apologize for the naming scheme) file. 
The code is a for loop that iterates from the beginning of the list of players to the end and removes them from the list.
However, despite running through the entire List, after each pass some player remained, it usually removed about half of the players.
Perhaps it needed a delay in between each pass of the loop to finish destroying the player object and removing it from the list, but this is something unity should have accomplished automatically.
The solution I found that worked quite well was to simple put the for loop inside a while loop whose condition was while list.Count!=0
As result it did not break out until all players had been destroyed
This was an extremely unpleasant solution that would not have been encountered if I had made this in pygame or other similar game framework, but to be fair those would have required more work to just get the game working, so it is a trade off.

Congrats on making it this far down! If you like the project stars are aprecciated!
