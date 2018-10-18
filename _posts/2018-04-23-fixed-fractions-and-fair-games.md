---
layout: post
title: Fixed Fractions and Fair Games
---

A gambler has a $100 bankroll. He's feeling lucky and he wants to make some bets. But he only wants to play fair games where the expectation is breakeven for a large number of plays. If the gambler plays a fair game repeatedly using a constant bet amount, would it still be a fair game if he decides to bet a fixed fraction of his bankroll instead of betting a fixed constant amount?

<!--excerpt-->

## Coin Toss Game

Suppose the gambler wants to place bets on the outcome of a coin toss. Ignoring the small possibility of the coin landing on its edge, there are only two possible outcomes: heads or tails. We can model a repeated coin toss game using the following notation:

{% latex 01 %}
    \begin{aligned}
    & \Omega = \{\ H,\ T\ \}
    \\[1em]
    &
    \begin{aligned}
    & X_i && \text{ is a random variable on } \Omega \text{ representing the } i \text{th toss of the coin}
    \\
    & V_0 && \text{ is the gambler's initial bankroll}
    \\
    & V_n && \text{ is the gambler's bankroll after } n \text{ plays }
    \\
    & b   && \text{ is a scaling factor that determines the bet size }
    \end{aligned}
    \end{aligned}
{% endlatex %}

If we assume it's a fair coin, then each of the two outcomes has a 50% probability of occurring for each toss of the coin:

{% latex 02 %}
    \setlength{\arraycolsep}{1em}
    \begin{array}{@{\rule{0em}{1.25em}}|l|l|}
    \hline
    X & Pr(X)
    \\[0.25em]\hline
    H & 0.5
    \\[0.25em]\hline
    T & 0.5
    \\[0.25em]\hline
    \end{array}
{% endlatex %}

The gambler needs to decide how much to bet on each play. If he bets his entire stake, then he risks losing everything on the first round. If he only bets a portion of his bankroll, then he can afford to absorb a few losses without going broke. There are two betting strategies the gambler can choose from: a fixed constant bet size strategy and a fixed fraction bet size strategy. The gambler must choose one strategy and stick to it for every round of the game.

## Fixed Constant Bet Size

Using the fixed constant bet size strategy, the gambler must choose a specific amount he wants to wager on each play. Suppose the gambler wants to bet $20 on each round. We can model this as 20% of the gambler's initial bankroll:

{% latex 03 %}
    b = 0.2
{% endlatex %}

Now let's define a reward function to determine the payoff for each round based on the outcome of the coin toss:

{% latex 04 %}
    R(X) =
    \begin{cases}
    + bV_0 & \quad \text{if } X = H
    \\
    - bV_0 & \quad \text{if } X = T
    \end{cases}
{% endlatex %}

The gambler gains $20 if the coin lands on heads; he loses $20 if the coin lands on tails. Is this a fair game? The answer might seem obvious, but let's make some empirical observations just to be sure. We can use the reward function above in the following equation to compute the gambler's holdings for a series of coin tosses:

{% latex 05 %}
    V_n = V_0 + \sum_{i = 1}^{n} R(X_i)
{% endlatex %}

Using a random number generator, we can run a computer simulation of the coin toss game and plot the gambler's bankroll after each round. Here is a plot of the gambler's holdings over a period of 200 plays using a random sequence of coin tosses:

{% chart fig-06-constant-add-lin-sim.svg %}

It looks like it just zigzags up and down randomly with no clear pattern. Empirically, this doesn't tell us much. If we run 10,000 unique simulations like the one above and then take the average value for each play, here is the result that emerges:

{% chart fig-07-constant-add-lin-avg.svg %}

The value appears to have a steady mean of $100, suggesting that this is indeed a fair game with a breakeven expectation. Taking the median value for each round produces a similar result:

{% chart fig-08-constant-add-lin-med.svg %}

The median value straddles the breakeven value of $100, lending further evidence that this is in fact a fair game. But can we determine the expected outcome analytically? Consider the arithmetic mean as the number of plays approaches infinity:

{% latex 09 %}
    A = \lim_{n \to \infty} \left[ V_0 + \frac{1}{n} \sum_{i = 1}^{n} R(X_i) \right]
{% endlatex %}

If we know the probability of each outcome of a coin toss, then we can use the law of large numbers to determine the number of winning games and losing games for a large number of plays. Let's use the following notation:

{% latex 10 %}
    \begin{aligned}
    & W  &&\text{ is the number of winning games}
    \\
    & L  &&\text{ is the number of losing games}
    \end{aligned}
{% endlatex %}

Instead of taking the limit as the number of plays approaches infinity, we can compute the arithmetic mean using the number of winning and losing games based on the expected behavior of a coin toss. As demonstrated below, the arithmetic mean is equal to the gambler's initial bankroll; a result that corresponds to the empirical observations derived from the simulations:

{% latex 11 %}
    \begin{aligned}
    A &= V_0 + \frac{1}{n} \bigg[ W(+bV_0) + L(-bV_0) \bigg]
    \\[1em]
      &= V_0 + \frac{1}{2}(+bV_0) + \frac{1}{2}(-bV_0)
    \\[1em]
      &= V_0
    \end{aligned}
{% endlatex %}

While this is certainly a fair game, look closely at the result of the repeated coin toss simulation. The gambler's holdings dip below zero for a few rounds before moving back up. This might not be possible if the gambler is unable to borrow a few bucks to temporarily cover his debt. Also note that the constant bet size is a smaller percentage of the gambler's stake as his winnings increase, but it's a larger percentage of his stake as his holdings decrease. What happens if he bets a fixed percentage every time?

## Fixed Fraction Bet Size

Using the fixed fraction bet size strategy, the gambler chooses a fixed percentage of his bankroll to wager on each play. Suppose the gambler wants to bet 20% on each round:

{% latex 12 %}
    b = 0.2
{% endlatex %}

Like the previous example, we can define a reward function to determine the payoff for each round based on the outcome of the coin toss:

{% latex 13 %}
    R(X) =
    \begin{cases}
    1 + b & \quad \text{if } X = H
    \\
    1 - b & \quad \text{if } X = T
    \end{cases}
{% endlatex %}

In this case, the reward function returns a multiplier that gets applied to the gambler's bankroll after each round. The following shows how to apply the reward function for a series of coin toss games:

{% latex 14 %}
    V_n = V_0 \prod_{i = 1}^{n} R(X_i)
{% endlatex %}

Using the same sequence of coin tosses from the previous example, we can simulate the coin toss game again using the fixed fraction bet size strategy. Here is a plot of the gambler's holdings over the same 200 coin tosses using fixed fraction bets:

{% chart fig-15-fraction-add-lin-sim.svg %}

It looks like it bounces up and down with smaller and smaller spikes until it ultimately fizzles out to zero. Here is the same plot on a logarithmic scale:

{% chart fig-16-fraction-add-log-sim.svg %}

On the logarithmic chart, the plot looks similar to that of the game played with a fixed constant bet size, only with a slight tilt downwards. Does this indicate a downward bias when playing with the fixed fraction bet size strategy? Let's see what happens if we take the average value of 10,000 simulations:

{% chart fig-17-fraction-add-lin-avg.svg %}

As with the previous example, the value appears to have a mean of $100. Notice, however, that the value seems to become increasingly unstable as more rounds are played. Taking the median value for each round produces a very different result:

{% chart fig-18-fraction-add-lin-med.svg %}

Unlike the previous example, the median value decays steadily downward, gradually approaching the zero asymptote. Here it is again on a logarithmic chart:

{% chart fig-19-fraction-add-log-med.svg %}

So is this a fair game or not? The mean value suggest that it might be, but the median value suggests otherwise. What if we take an analytical approach? Since the fixed fraction betting strategy is multiplicative instead of additive, let's consider the geometric mean instead of the arithmetic mean:

{% latex 20 %}
    G = \lim_{n \to \infty} \left[ V_0 \left( \prod_{i = 1}^{n} R(X_i) \right)^\frac{1}{n} \right]
{% endlatex %}

Using the law of large numbers, we can apply the expected number of winning and losing games as we did before in the previous example. The following calculation provides an explanation for the median values observed in the simulation results:

{% latex 21 %}
    \begin{aligned}
    G &= V_0 \bigg[ (1 + b)^W (1 - b)^L \bigg]^\frac{1}{n}
    \\[1em]
      &= V_0 \bigg[ (1 + b) (1 - b) \bigg]^\frac{1}{2}
    \\[1em]
      &= V_0 \sqrt{0.96}
    \end{aligned}
{% endlatex %}

The geometric mean computed above indicates that the gambler's bankroll is expected to be a fraction of its previous value after each play, leading to a conclusion that the gambler is at a disadvantage when playing with a fixed fraction betting strategy. But how can we account for the breakeven average values we observed in the empirical results? Perhaps we can get a better understanding if we examine the possible outcomes more closely.

## Understanding the Results

What exactly does it mean for a gambling game to be a fair game with a breakeven expectation? As we saw with the fixed fraction betting game, the answer can be somewhat ambiguous. There are two aspects of the coin toss game that are worth looking at:

* The average value of the gambler's bankroll for all possible outcomes
* The percentage of winning, losing, and breakeven outcomes

Let's suppose the gambler plays two rounds of the coin toss game and always bets on heads. There are four possible outcomes, each with an equal probability of occurring. If the gambler chooses the fixed constant betting strategy, the value of the gambler's stake after each possible outcome is shown below:

{% latex 22 %}
    \setlength{\arraycolsep}{1em}
    \begin{array}{@{\rule{0em}{1.25em}}|l|l|}
    \hline
    \text{Outcome} & \text{Value}
    \\[0.25em]\hline
    HH & \$140
    \\[0.25em]\hline
    HT & \$100
    \\[0.25em]\hline
    TH & \$100
    \\[0.25em]\hline
    TT & \$60
    \\[0.25em]\hline
    \end{array}
  {% endlatex %}

The average value of all possible outcomes in this case is $100, which is the breakeven amount. Also notice that there are an equal number of winning games as there are losing games. Both aspects are indicative of a fair game. If the gambler chooses the fixed fraction betting strategy, the results are a bit different:

{% latex 23 %}
    \setlength{\arraycolsep}{1em}
    \begin{array}{@{\rule{0em}{1.25em}}|l|l|}
    \hline
    \text{Outcome} & \text{Value}
    \\[0.25em]\hline
    HH & \$144
    \\[0.25em]\hline
    HT & \$96
    \\[0.25em]\hline
    TH & \$96
    \\[0.25em]\hline
    TT & \$64
    \\[0.25em]\hline
    \end{array}
{% endlatex %}

As with the fixed constant betting strategy, the average value of all possible outcomes is $100. The fixed fraction game is a fair game in this respect. Now look at the number of winning outcomes versus the number of losing outcomes. Only 25% of the outcomes are winners, while 75% of the outcomes result in a loss. The player is three times more likely to have a losing outcome than have a winning outcome, which is not a fair expectation at all. But while a losing outcome is more likely, the payoff of a winning outcome is large enough to offset the other three losing outcomes combined.
