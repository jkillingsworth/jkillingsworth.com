---
layout: post
title: Using the Kelly Criterion for Optimal Bet Sizing
---

Suppose a gambler is playing a game in which he has a statistical advantage. And let's assume he can quantify his advantage with a fair amount of accuracy. If the gambler plays this game over and over again, what percentage of his bankroll should he bet on each round if he wants to maximize his winnings? In this post,  I explore this question using a series of examples illustrating the application of the Kelly criterion. Many of the ideas presented here are inspired by materials written by Ed Seykota, Edward O. Thorp, and J. L. Kelly.

<!--excerpt-->

## Repeated Coin Toss Game

For our first example, let's consider a repeated coin toss game. The gambler bets on the outcome of a coin toss. If he predicts the outcome correctly, he wins an amount equivalent to the size of his bet. If he is wrong, he loses the amount he wagered. In this example, let's assume the coin is biased in such a way that it lands on heads 60% of the time and tails 40% of the time. We can represent this using the following notation:

{% latex fig-01 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p & = 0.6
    \\
    q & = 0.4
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Let's assume the gambler is aware of this bias. Of course, the gambler will always bet on heads. But how much should he bet? Should he bet 10% of his stake? Should he bet 20%? Perhaps he should bet 30%? Maybe more? Maybe less? This is the question we want to answer. If the gambler bets too high, he risks depleting his capital too quickly during a losing streak. If he bets too low, his wealth may not grow as quickly as it might otherwise. We can being to answer this question by considering an example and then constructing a model. Suppose the gambler plays 200 rounds of the game and bets 10% on each play:

{% latex fig-02 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    n & = 200
    \\
    b & = 0.1
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

And suppose the gambler starts with a bankroll of $100:

{% latex fig-03 %}
    \begin{document}
    \begin{displaymath}
    V_0 = 100
    \end{displaymath}
    \end{document}
{% endlatex %}

Based on these numbers, we can use the following formula to calculate the size of the gambler's holdings after playing 200 rounds of the game:

{% latex fig-04 %}
    \begin{document}
    \begin{displaymath}
    V_n = V_0 \1 \brace2[]{ (1 + b)^H (1 - b)^T }
    \end{displaymath}
    \end{document}
{% endlatex %}

Because there is an element of randomness involved, there is no guarantee that we will get the same result every time we compute this value. One series of 200 coin tosses might have a different number of heads and tails than another series. However, the number of heads and the number of tails will always add up to the same amount:

{% latex fig-05 %}
    \begin{document}
    \begin{displaymath}
    n = H + T
    \end{displaymath}
    \end{document}
{% endlatex %}

To get an idea of what kind of results we can expect, we can run some simulations of the coin toss game using a random number generator to mimic the coin toss. Here is a graphical representation of three such simulations:

{% chart fig-06-coin-toss-simulations.svg %}

As you can see, these three different simulations yield three very different outcomes. The following table summarizes the results:

{% latex fig-07 %}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{5em}|wr{2em}|wr{2em}|wr{4em}|wr{4em}|}
    \hline
    \text{Simulation} & H & T & V_0 & V_n
    \\[0.25em]\hline
    \text{A} & 107 &  93 & 100.00 & \text{149.13}
    \\[0.25em]\hline
    \text{B} & 120 &  80 & 100.00 & \text{2,025.46}
    \\[0.25em]\hline
    \text{C} & 117 &  83 & 100.00 & \text{1,109.36}
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

To estimate the expected value of the gambler's holdings after 200 rounds of the coin toss game, we might run this simulation many more times and then take the mean of the results. That would tell us the expected value for a bet size of 10%. We would then have to repeat this process again for every possible bet size to determine which bet size value gives us the highest expected result. That might be a computationally expensive thing to do. A better approach might be to look at this problem analytically. Keep in mind that what we really care about here is not the gambler's wealth after a specific number of rounds of the coin toss game. What really matters is the geometric mean of all the gains and losses taken in aggregate:

{% latex fig-08 %}
    \begin{document}
    \begin{displaymath}
    F = \brace3(){ \frac{V_n}{V_0} }^{\frac{1}{n}}
    \end{displaymath}
    \end{document}
{% endlatex %}

This gives us a growth rate factor independent of the number of plays and the independent of the gambler's starting equity. This is the value we want to maximize. And specifically, what we're really interested in is the expected value of the growth rate as a function of the bet size:

{% latex fig-09 %}
    \begin{document}
    \begin{displaymath}
    f(b) = \mathbb{E}\1\brace1[]{ F(b) }
    \end{displaymath}
    \end{document}
{% endlatex %}

As we will see shortly, maximizing the logarithm of the growth rate is going to be more convenient than maximizing the growth rate itself. For our purposes here, we will use the natural logarithm. Consider the following:

{% latex fig-10 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    G & = \log\2\brace3(){ \frac{V_n}{V_0} }^{\frac{1}{n}}
    \\[1em]
      & = \tfrac{1}{n} \log\2\brace2[]{ (1 + b)^H (1 - b)^T }
    \\[1em]
      & = \tfrac{H}{n} \log\2(1 + b) + \tfrac{T}{n} \log\2(1 - b)
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Since the natural logarithm is a monotonically increasing function, maximizing the growth rate is the same as maximizing its logarithm. As mentioned, we are interested in the expected value of the growth rate. But in this case, it is the logarithm of the growth rate:

{% latex fig-11 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    g(b) & = \mathbb{E}\1\brace1[]{ G(b) }
    \\[1em]
         & = p \log\2(1 + b) + q \log\2(1 - b)
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

You can think about the expected value in terms of the number of heads and the number of tails. If you play 200 rounds, the expectation is that the game will produce 120 heads and 80 tails. This can vary based on randomness, but these are the expected values. We can plot the function above to see what it looks like:

{% chart fig-12-example-1-gs.svg %}

Using visual inspection, we can observe that the curve peaks where the bet size is 20%. But how do we solve this analytically? We want to be able to compute the most optimal bet size. Let's use the following notation to represent the most optimal bet size:

{% latex fig-13 %}
    \begin{document}
    \begin{displaymath}
    b^* = \operatorname*{argmax}_{b}\2\brace1{\lbrace}{\rbrace}{\, g(b) \,}
    \end{displaymath}
    \end{document}
{% endlatex %}

The derivative of the growth rate function is zero at the maximum, so we can say this:

{% latex fig-14 %}
    \begin{document}
    \begin{displaymath}
    g'(b^*) = 0
    \end{displaymath}
    \end{document}
{% endlatex %}

Expanding out the derivative of the growth rate function, we get this:

{% latex fig-15 %}
    \begin{document}
    \begin{displaymath}
    g'(b) = \frac{p}{1 + b} - \frac{q}{1 - b}
    \end{displaymath}
    \end{document}
{% endlatex %}

Setting the derivative to zero, we can solve for the optimal bet size:

{% latex fig-16 %}
    \begin{document}
    \begin{displaymath}
    b^* = \frac{p - q}{p + q}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can simplify this further knowing that the sum of the probabilities adds up to one:

{% latex fig-17 %}
    \begin{document}
    \begin{displaymath}
    p + q = 1
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is the simplified solution:

{% latex fig-18 %}
    \begin{document}
    \begin{displaymath}
    b^* = p - q
    \end{displaymath}
    \end{document}
{% endlatex %}

And now, plugging in the probability values that quantify the bias of the coin, we can compute the answer to our original question:

{% latex fig-19 %}
    \begin{document}
    \begin{displaymath}
    b^* = 0.2000
    \end{displaymath}
    \end{document}
{% endlatex %}

Thus, our gambler would maximize his winnings by betting 20% of his current bankroll for each round of the coin toss game. We can even take it a step further and compute his expected growth for rate for a single round of the game. Consider the relationship between the original growth rate function and its logarithmic equivalent:

{% latex fig-20 %}
    \begin{document}
    \begin{displaymath}
    f(b) = \exp\2\brace1[]{ g(b) }
    \end{displaymath}
    \end{document}
{% endlatex %}

Using the optimal bet size, we can calculate the optimal growth rate values:

{% latex fig-21 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    g(b^*) & = 0.0201
    \\
    f(b^*) & = 1.0203
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

So now, if our gambler bets 20% of his stake each round, he can expect to increase his wealth by 2.03% every time he plays. Of course, this is just an expectation. The actual result will vary every round based on the randomness of the game.

## Different Payoffs

In the previous example, the winning and losing amounts were the same. In this example, we are going to see what happens when there is an asymmetric payoff rate. To make things interesting, we will start with a coin that only lands on heads 30% of the time and tails for the remaining 70% of the time:

{% latex fig-22 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p & = 0.3
    \\
    q & = 0.7
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

As with the previous example, the gambler always bets on heads. You might think this would be a losing proposition since there will be more losing rounds than winning rounds. But if the payoff for a winning round is higher than the loss suffered on a losing round, the gambler still might have an advantage when betting on heads. In this case, let's say the gain is four times the loss:

{% latex fig-23 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    u & = +4
    \\
    v & = -1
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

If the gambler chooses a bet size of $1, he will only lose $1 if the coin lands on tails. But if the coin lands on heads, he will gain $4. This is enough to offset the higher loss rate. We can now update our model of the game to account for this type of asymmetric payoff:

{% latex fig-24 %}
    \begin{document}
    \begin{displaymath}
    V_n = V_0 \1 \brace2[]{ (1 + bu)^H (1 + bv)^T }
    \end{displaymath}
    \end{document}
{% endlatex %}

So now the question is, how much should the gambler bet? What percentage of his holdings should he wager if he wants to maximize his winnings? We can do the same analysis as before to come up with the logarithmic growth rate as a function of the bet size:

{% latex fig-25 %}
    \begin{document}
    \begin{displaymath}
    g(b) = p \log\2(1 + bu) + q \log\2(1 + bv)
    \end{displaymath}
    \end{document}
{% endlatex %}

This is similar to the growth rate function in the previous example. Except now, we have taken into consideration the asymmetric payoffs. Here is a plot of this function:

{% chart fig-26-example-2-gs.svg %}

Like we did in the previous example, we can find the maximum growth rate by finding the point at which the derivative of the growth rate function equals zero. Here is the derivative of the growth rate function:

{% latex fig-27 %}
    \begin{document}
    \begin{displaymath}
    g'(b) = \frac{pu}{1 + bu} + \frac{qv}{1 + bv}
    \end{displaymath}
    \end{document}
{% endlatex %}

Settings the derivative to zero and solving for the bet size, we can arrive at the optimal bet size that maximizes the growth rate:

{% latex fig-28 %}
    \begin{document}
    \begin{displaymath}
    b^* = -\frac{pu + qv}{uv}
    \end{displaymath}
    \end{document}
{% endlatex %}

Plugging in the values, we get the concrete solution:

{% latex fig-29 %}
    \begin{document}
    \begin{displaymath}
    b^* = 0.1250
    \end{displaymath}
    \end{document}
{% endlatex %}

And now we can evaluate the growth rate function at the optimal bet size:

{% latex fig-30 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    g(b^*) & = 0.0282
    \\
    f(b^*) & = 1.0286
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

According to these calculations, if the gambler uses the optimal bet size of 12.5%, he can expect to gain 2.86% every time he plays. Despite losing more often than he wins, the gambler can still make a profit under the conditions of this particular game.

## Multiple Outcomes

This example is a generalization of the previous ones. Instead of using a coin toss, here we consider a betting game in which there are six possible outcomes. Each outcome has a different probability. And each outcome has a different payoff rate. The following table shows the payoffs and probabilities of each outcome:

{% latex fig-31 %}
    \begin{document}
    \begin{displaymath}
    \begin{table}{|wl{4em}|wl{5em}|wl{3em}|}
    \hline
    \text{Outcome} & \text{Probability} & \text{Payoff}
    \\[0.25em]\hline
    1 & 10\% & +3
    \\[0.25em]\hline
    2 & 10\% & +2
    \\[0.25em]\hline
    3 & 20\% & +1
    \\[0.25em]\hline
    4 & 30\% & \phantom{\pm}0
    \\[0.25em]\hline
    5 & 20\% & -1
    \\[0.25em]\hline
    6 & 10\% & -2
    \\[0.25em]\hline
    \end{table}
    \end{displaymath}
    \end{document}
{% endlatex %}

For our analysis, let's use the following notation to represent the payoff matrix above:

{% latex fig-32 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = 0.1 & \quad w_1 & = +3
    \\
    p_2 & = 0.1 & \quad w_2 & = +2
    \\
    p_3 & = 0.2 & \quad w_3 & = +1
    \\
    p_4 & = 0.3 & \quad w_4 & = \phantom{\pm}0
    \\
    p_5 & = 0.2 & \quad w_5 & = -1
    \\
    p_6 & = 0.1 & \quad w_6 & = -2
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Notice that, like in the previous examples, the sum of the probabilities adds up to one:

{% latex fig-33 %}
    \begin{document}
    \begin{displaymath}
    \sum_{i = 1}^{6} p_i = 1
    \end{displaymath}
    \end{document}
{% endlatex %}

Since there is a 10% chance that we can lose twice the bet size, there is an implicit hard limit on the maximum possible bet size at 50%. We will address these kinds of limitations in a later section. For now, here is the growth rate function that we want to maximize:

{% latex fig-34 %}
    \begin{document}
    \begin{displaymath}
    g(b) = \sum_{i = 1}^{6} p_i \log\2(1 + b w_i)
    \end{displaymath}
    \end{document}
{% endlatex %}

While there are six possible outcomes in this example, there can be an arbitrary number of possible outcomes in the general case. Because of this, it is best to represent the growth rate function as a summation. Here is a plot of the growth rate function:

{% chart fig-35-example-3-gs.svg %}

Now at this point, we might try the same approach that we did before in the last two examples. We can take the derivative of the growth rate function and then find the point at which the derivative is equal to zero. But look at this derivative:

{% latex fig-36 %}
    \begin{document}
    \begin{displaymath}
    g'(b) = \sum_{i = 1}^{6} \frac{p_i w_i}{1 + b w_i}
    \end{displaymath}
    \end{document}
{% endlatex %}

Can you solve this? Go ahead and try it. But it might just be easier to use a numerical root-finding method. And if you're going to use a numerical method, you might also consider using a numerical optimization method on the growth rate function itself instead of bothering with the derivative. I decided to use the Nelder--Mead method implemented by a third-party library to find the bet size value that maximizes the growth rate function. Here is the result:

{% latex fig-37 %}
    \begin{document}
    \begin{displaymath}
    b^* = 0.1571
    \end{displaymath}
    \end{document}
{% endlatex %}

And now, we can plug this value into the growth rate function:

{% latex fig-38 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    g(b^*) & = 0.0232
    \\
    f(b^*) & = 1.0235
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

For this game, the optimal bet size is 15.71%. The gambler can expect to grow his wealth by 2.35% every round if he plays the optimal bet size. Notice that, in this case, unlike the last two examples, the bet size value does not represent the maximum amount the gambler stands to lose. Instead, it is simply a multiplier or a scaling factor. Based on the payoff matrix for this particular example, the gambler can lose up with twice the bet size on any given round.

## Stock Trading

Now let's see how to apply this optimal bet sizing technique to stock trading. You can think of stock market trading as a form of gambling. That is the best analogy. Suppose the gambler---or the trader, if you want to call him that---speculates that the price of a stock will close at one of six possible price points after a specific amount of time:

{% latex fig-39 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = 0.1 & \quad x_1 & = 23
    \\
    p_2 & = 0.1 & \quad x_2 & = 22
    \\
    p_3 & = 0.2 & \quad x_3 & = 21
    \\
    p_4 & = 0.3 & \quad x_4 & = 20
    \\
    p_5 & = 0.2 & \quad x_5 & = 19
    \\
    p_6 & = 0.1 & \quad x_6 & = 18
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The price points range from as low as $18 per share to as high as $23 per share, and the gambler's hypothesis associates different probabilities with each outcome. Let's assume that the stock has a current market value of $20 per share:

{% latex fig-40 %}
    \begin{document}
    \begin{displaymath}
    c = 20
    \end{displaymath}
    \end{document}
{% endlatex %}

This is the price point at which the trader can enter into a position. To determine the payoff rate for each possible outcome, we can use a payoff function that takes the entry price into consideration. Here is the payoff function:

{% latex fig-41 %}
    \begin{document}
    \begin{displaymath}
    w(x) = \frac{x - c}{c}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here are the computed payoff values for each one of the possible outcomes:

{% latex fig-42 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    w(x_1) & = +0.15
    \\
    w(x_2) & = +0.10
    \\
    w(x_3) & = +0.05
    \\
    w(x_4) & = \phantom{\pm}0.00
    \\
    w(x_5) & = -0.05
    \\
    w(x_6) & = -0.10
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is the growth rate function that we want to maximize:

{% latex fig-43 %}
    \begin{document}
    \begin{displaymath}
    g(b) = \sum_{i = 1}^{6} p_i \log\2\brace1[]{ 1 + b w(x_i) }
    \end{displaymath}
    \end{document}
{% endlatex %}

Notice how the growth rate function uses the payoff function inside the summation. Plotting this function on a chart, here is what it looks like:

{% chart fig-44-example-4-gs.svg %}

As with the previous example, we can use a numerical optimization method to find the most optimal bet size that maximizes the growth rate. Here is the result:

{% latex fig-45 %}
    \begin{document}
    \begin{displaymath}
    b^* = 3.1422
    \end{displaymath}
    \end{document}
{% endlatex %}

This value is greater than one. What does this mean? It means that our trader would have to use leverage if he wants to take the most optimal position in this stock. A trader with a $10,000 account would have to buy $31,422 worth of this stock---about 1,571 shares. Using this amount of leverage may or may not be possible. Assuming it is, here is the expected growth rate:

{% latex fig-46 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    g(b^*) & = 0.0232
    \\
    f(b^*) & = 1.0235
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Notice the similarity to the previous example. In fact, this is a repeat of the previous example. The only difference is that the payoff values are scaled down by a factor of twenty. And this results in the optimal bet size being scaled up by a factor of twenty. But the optimal growth rate remains the same for both examples.

## Continuous Distributions

The previous example used what is basically a discrete probability mass function to model the projected behavior of stock price movements. Since stock prices can move along a continuous range of values, it might be more realistic to use a continuous model instead of a discrete model. In this example, we will use a log-normal probability density function:

{% latex fig-47 %}
    \begin{document}
    \begin{displaymath}
    p(x)
    =
    \frac{1}{x \sigma \sqrt{2 \pi}} \exp\2\brace4[]{ -\frac{(\log x - \mu)^2}{2 \sigma^2} }
    \end{displaymath}
    \end{document}
{% endlatex %}

This function ranges from zero to infinity, and the area under the curve is one:

{% latex fig-48 %}
    \begin{document}
    \begin{displaymath}
    \int_{0}^{\infty} p(x) \, \dderiv x = 1
    \end{displaymath}
    \end{document}
{% endlatex %}

For a log-normal probability density function, we need to provide appropriate values for the mean and the standard deviation. How to come up with these values is beyond the scope of this discussion, so we will just make up some fictitious numbers for the sake of example:

{% latex fig-49 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mu    & = \log\2(20.25)
    \\
    \sigma & = \log\2(1.072)
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Using these values for the mean and standard deviation, we can plot the probability density function to see what it looks like. Here is the plot:

{% chart fig-50-example-5-ps.svg %}

This is our speculative forecast of what the stock price might be after a specific amount of time has passed. As you can see from looking at the chart, the price is most likely to fall somewhere between $15 per share and $25 per share. But there is a small chance that it could fall outside this range as well. For this example, we will assume the current price at which we can enter into a position is $20 per share, and we will bound the exit point to somewhere between $17 per share and $23 per share:

{% latex fig-51 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    c & = 20
    \\
    r & = 17
    \\
    s & = 23
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

We are assuming idealized exit point boundaries here. We can never gain or lose more than $3 per share. In reality, enforcement of this boundary condition might take the form of a stop-loss order at $17 and a take-profit order at $23. Or it might also take the form of a long position in a put option with a strike price of $17 and a short position in a call option with a strike price of $23, disregarding any premiums paid or received. Using these values, here is our payoff function:

{% latex fig-52 %}
    \begin{document}
    \begin{displaymath}
    w(x) =
    \begin{dcases}
    \frac{r - c}{c} & \quad \text{if $x < r$}
    \\[0.5em]
    \frac{s - c}{c} & \quad \text{if $x > s$}
    \\[0.5em]
    \frac{x - c}{c} & \quad \text{otherwise}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

This payoff function is a piecewise function that limits the loss and caps the gain at the exit point boundaries. While this function is not necessarily differentiable at all points, note that it is continuous at all points. Here is a graphical representation of this function:

{% chart fig-53-example-5-ws.svg %}

Given the continuous probability density function and the continuous payoff function, we can now compute the expected growth rate as a function of the bet size. Instead of using a summation as we did previously, in this example, we will use an integral:

{% latex fig-54 %}
    \begin{document}
    \begin{displaymath}
    g(b) = \int_{0}^{\infty} p(x) \0 \log\2\brace1[]{ 1 + b w(x) } \, \dderiv x
    \end{displaymath}
    \end{document}
{% endlatex %}

This is the growth rate function that we want to maximize. Since the payoff function used here is a piecewise function, we might want to break this integral up into three different partitions:

{% latex fig-55 %}
    \begin{document}
    \begin{displaymath}
    g(b) =
    \int_{0}^{\mathrlap{r}} \dots \, \dderiv x
    +
    \int_{r}^{\mathrlap{s}} \dots \, \dderiv x
    +
    \int_{s}^{\mathrlap{\infty}} \dots \, \dderiv x
    \end{displaymath}
    \end{document}
{% endlatex %}

I have not figured out how to boil this integral down to an expression in analytic form. In particular, it is the middle partition I have not been able to find the solution for. However, this problem can be solved using numerical integration. In this case, I am using the Gauss--Legendre method to find an approximation. Here is what this function looks like on a chart:

{% chart fig-56-example-5-gs.svg %}

Using numerical optimization on top of numerical integration, we can compute the optimal bet size that maximizes the growth rate function. Here is the result:

{% latex fig-57 %}
    \begin{document}
    \begin{displaymath}
    b^* = 2.9866
    \end{displaymath}
    \end{document}
{% endlatex %}

Just like the previous example, this is a leveraged position. Here is the maximum growth rate using the optimal bet size:

{% latex fig-58 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    g(b^*) & = 0.0211
    \\
    f(b^*) & = 1.0213
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Admittedly, this example is a bit idealistic. It assumes that the trader can repeat the same bet again and again. But in real life, market conditions can change over time. The price point where the trader can enter a position can change as the tide of the market goes up and down, changing the payoff function. And the probability distribution of the projected price movements can change as well. In fact, the price movements might be difficult to model accurately in the first place.

## Short Positions

The optimal bet size does not always have to be a positive number. Sometimes it can be a negative number. A negative number can indicate that the bet on offer is a losing bet that is not worth taking. It might also represent an opportunity for profit if the gambler is able to take the other side of the bet. For a stock trader, a negative value indicates that the most advantageous thing to do would be to take a short position. Let's repeat the previous example using a slightly different probability density function. We'll use the following mean and standard deviation values:

{% latex fig-59 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mu    & = \log\2(19.65)
    \\
    \sigma & = \log\2(1.076)
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is a plot of the log-normal probability density function using these values:

{% chart fig-60-example-6-ps.svg %}

The peak of the curve is shifted a bit to the left compared to the density function used in the previous example. For this example, we are going to use the same entry price and exit point boundaries that we used in the previous example:

{% latex fig-61 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    c & = 20
    \\
    r & = 17
    \\
    s & = 23
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

As you might expect, the payoff function is exactly the same as well:

{% chart fig-62-example-6-ws.svg %}

The expected value of the growth rate as a function of the bet size looks like this:

{% chart fig-63-example-6-gs.svg %}

Notice the maximum value appears on the negative side of the vertical axis this time around. Computing the optimal bet size numerically, this is the result:

{% latex fig-64 %}
    \begin{document}
    \begin{displaymath}
    b^* = -2.8563
    \end{displaymath}
    \end{document}
{% endlatex %}

It is a negative number, indicating that the trader should sell the stock short. Here is the expected growth rate calculation if the trader takes the optimally sized short position:

{% latex fig-65 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    g(b^*) & = 0.0217
    \\
    f(b^*) & = 1.0219
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

I think it's worth mentioning here that this result is the polar opposite of the optimal bet size value found in the previous example. This is surprising because these two examples are nearly identical. The only difference is in the mean and standard deviation values used in the probability density functions. And the difference isn't that great. The two probability densities are not that much different from one another. I think this illustrates how sensitive this bet size optimization technique is to the accuracy of the model used to quantify the game.

## Constraints and Limitations

As alluded to earlier when discussing multiple outcomes, there are hard limits to how much a gambler can bet or how much leverage a trader can use. When using the Kelly criterion, if there is any chance at all, no matter how small it might be, that the gambler can lose his entire stake on any one round of the game, then the expected growth rate is zero. This is the extreme case of the gambler betting an amount higher in magnitude than the optimal bet size. In the stock trading model illustrated in the last two examples, we must take these limits into consideration when allowing the trader to take a leveraged position. Suppose a trader takes a position in a stock at an entry price of $20 per share:

{% latex fig-66 %}
    \begin{document}
    \begin{displaymath}
    c = 20
    \end{displaymath}
    \end{document}
{% endlatex %}

Setting the lower exit point value to $15 per share would constrain the maximum bet size for a long position to four times the trader's account value. If the trader were to lose more than $5 per share with this amount of leverage, his account would go into negative territory. Likewise, taking a short position with this amount of leverage would constrain the upper exit point value to no more than $25 per share. Here is how to calculate the exit point limits in relation to the bet size:

{% latex fig-67 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mathrlap{s_\sscript{max}}\phantom{s_\sscript{max}} & =
    \begin{dcases}
    \frac{c \1 (b - 1)}{b} & \quad \text{if $b < 0$}
    \\[0.5em]
    +\infty                & \quad \text{if $b \geq 0$}
    \end{dcases}
    \\[1em]
    \mathrlap{r_\sscript{min}}\phantom{s_\sscript{max}} & =
    \begin{dcases}
    \frac{c \1 (b - 1)}{b} & \quad \text{if $b > 1$}
    \\[0.5em]
    0                      & \quad \text{if $b \leq 1$}
    \end{dcases}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

For a long position, there is effectively no limit to the upper exit point. It can go to infinity. And for a short position, the lower exit point can go all the way to zero without putting any restrictions on the bet size. In the case of an unleveraged long position, both the lower and the upper exit points can be unlimited. Here is a plot of the formulas above to give a visual illustration:

{% chart fig-68-limitations-1.svg %}

The solid lines show the hard limits that constrain the upper and lower exit points. The shaded regions show the valid areas where the exit points can be placed. But this doesn't tell the whole story. The stock trader's broker might have margin requirements that impose additional constraints on the above. Suppose the broker has a 50% initial margin requirement to enter into a position and a 25% maintenance margin requirement:

{% latex fig-69 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mathrlap\ell\phantom{m} & = 0.50
    \\
    m & = 0.25
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The initial margin requirement puts a cap on the magnitude of the bet size. The allowable range of bet size values is inversely proportional to the initial margin requirement:

{% latex fig-70 %}
    \begin{document}
    \begin{displaymath}
    -\ell^{-1} \leq b \leq +\ell^{-1}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can also use the following notation to describe the extremes of the bet size values:

{% latex fig-71 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mathrlap{b_\sscript{max}}\phantom{b_\sscript{max}} & = +\ell^{-1}
    \\
    \mathrlap{b_\sscript{min}}\phantom{b_\sscript{max}} & = -\ell^{-1}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Taking the maintenance margin requirement into consideration, we can now revise the formulas for the exit point limits:

{% latex fig-72 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mathrlap{s_\sscript{max}}\phantom{s_\sscript{max}} & =
    \begin{dcases}
    \frac{c \1 (b - 1)}{b \1 (1 + m)} & \quad \text{if $b < 0$ and $b \geq b_\sscript{min}$}
    \\[0.5em]
    +\infty                           & \quad \text{if $b \geq 0$ and $b \leq b_\sscript{max}$}
    \end{dcases}
    \\[1em]
    \mathrlap{r_\sscript{min}}\phantom{s_\sscript{max}} & =
    \begin{dcases}
    \frac{c \1 (b - 1)}{b \1 (1 - m)} & \quad \text{if $b > 1$ and $b \leq b_\sscript{max}$}
    \\[0.5em]
    0                                 & \quad \text{if $b \leq 1$ and $b \geq b_\sscript{min}$}
    \end{dcases}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The margin requirements add additional constraints both horizontally and vertically. Here is the revised visualization:

{% chart fig-73-limitations-2.svg %}

As you can see, the range of exit point values that would allow the trader to avoid a margin call is a subset of the range of values that can be considered under the Kelly criterion. To perform the optimal bet size analysis under these constraints, there are different approaches you can take. You might try setting the exit point values to the upper and lower limits for the most extreme bet size values and then performing the optimization procedure to find the most optimal bet size across all possible bet size values. Alternatively, you might choose the exit point values first based on some criteria and then limit the range of possible bet size values to those that allow for the chosen range of exit points.

## Psychology and Variances

In addition to the hard limits described above, there may also be some soft limits to how big the bet size can be. The focus of this study has been on finding the most optimal bet size, but we did not address the variance of possible outcomes for a repeated betting game. Even when there is a positive expectation, the gambler may experience losing streaks that cause him to second guess himself. Taking the most optimal bet size might be a psychologically difficult thing to do, and the gambler might be more comfortable taking a smaller bet. It might be worth doing a further study to understand how much variation the gambler might expect in the possible outcomes of playing a repeated game of chance.

{% accompanying_src_link %}
