# OthelloMillenium IA

Ce projet a pour la réalisation d'IA pour une variante du jeu Othello avec une grille de 7x9.

# Auteurs

- [Segan Salomon](https://github.com/Maering)
- [Bastien Wermeille](https://github.com/Ph0tonic)

## Algorithme

L'algorithme utilisé est alpha-beta, mais adapté afin de prendre en compte le fait de faire rater un tour à l'adversaire.

## Architecture globale

Pour l'implémentation de cet algorithme, nous utilisons une classe `TreeNode` permettant de représenter un noeud de l'arbre de recherche.

## Fonction d'évaluation

L'évaluation d'un board repose sur plusieurs principes clés:
1. Les coins sont très importants et ne doivent pas être perdus
2. Plus la mobilité d'un joueur est petite moins il aura de coups possible et donc plus on pourra orienter ses coups
3. Un pion dit "stable" ne peut pas être retourné

Nous avons utilisé la ["stratégie de la mobilité"](http://www.ffothello.org/strategie/livret-de-strategie/livret-3-strategie-i/) qui consiste à limiter le nombre de coups possible pour l'adversaire.

En plus d'utiliser ce principe, nous avons ajouté le fait que la prise d'un coin augmente le score, mais surtout que la perte d'un coin apporte un malus bien plus important que la prise d'un coin.

La fonction d'évaluation est le point central de l'IA et évolue durant la partie.

### Score de positionnement

La matrice pour le positionnement des points est la suivante:
```c#
{
    {100, -50,  8,  6,  6,  6,  8, -50,  100},
    {-50,-100, -4, -4, -4, -4, -4,-100,  -50},
    {  8,  -4,  6,  4,  4,  4,  6,  -4 ,   8},
    {  6,  -4,  4,  0,  0,  4,  4,  -4 ,   6},
    {  8,  -4,  6,  0,  0,  4,  6,  -4 ,   8},
    {-50,-100, -4, -4, -4, -4, -4, -100, -50},
    {100, -50,  8,  6,  6,  6,  8,  -50, 100}
}
```

On remarque que certains points possèdent un score négatif, car elles permettent de prendre des points très importants, ces cases vont ainsi être évitées le plus longtemps possible.

### Score de stabilité

La fonction permettant de compter le nombre de points stables a été simplifiée et ne prend en compte que les points dans les angles comme sur l'image ci-dessous:

![alt text](http://www.ffothello.org/images/jeu/livret-2/image-2.jpg "Points stables")

La stabilité est un indicateur très important permettant de savoir si un point peut être retourné ou pas. Plus nous possédons de points stables plus la probabilité de gagner augmente.

Dans le cas ou une ligne sur le bord du plateau de jeu est complète alors tous les points sont stables. Afin de simplifier la fonction de stabilité, nous ne gérons pas ce cas.

### Évaluation dynamique

Les différentes techniques précédemment citées ne permettent pas à elles seules de gagner une partie.

Comme la stratégie globale se déroule en plusieurs étapes, nous avons séparé la partie en 3 phases de jeu de même durée(1/3 chacune).

Puis pour chaque phase déterminée des coefficients pour les différentes heuristiques. Ces coefficients ont été obtenus par successions de tests.

1. Early-Game : 2000*Coins + 200*Placement + 600*Mobilité
2. Mid-Game : 2000*Coins + 250*Mobilité + 100*Stabilité
3. Late-game : 2000*Coins + 50*Mobilité + 100*Stabilité + 100*Mobilité

La mobilité apparait toujours en fin de partie dans le cas ou l'adversaire applique une stratégie consistant à n'offrir aucun angle.

### Cas particuliers pour fin de jeu

Dans le cas ou le jeu est ternminé le score correspond à la différence de points entre les deux joueurs multiplié par 10000. Celà permet ainsi d'éviter à tous pris une défaite mais également de cibler un eradication si elle est possible car le score de l'éradication sera plus grand que tous les autres.

## Liens utiles (Consulté le 17.02.2019)

http://pressibus.org/ataxx/autre/minimax/node3.html
http://www.samsoft.org.uk/reversi/strategy.htm#evaporate
http://www.ffothello.org/strategie/livret-de-strategie/livret-5-tactique/
