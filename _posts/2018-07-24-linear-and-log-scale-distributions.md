---
layout: post
title: Linear and Log Scale Distributions
---

In my previous post titled *Fixed Fractions and Fair Games*, I explored the properties of two different betting strategies applied to a repeated coin toss game. The focus was on the expected value for each of the two betting strategies. In this post, I take a deeper look at the distribution of possible outcomes after a large number of plays.

<!--excerpt-->

## Estimating the Distribution Empirically

Suppose a gambler with $100 plays 200 rounds of the coin toss game, betting a fixed constant of $20 on each play. The expectation is breakeven. The most likely outcome for the gambler is that he has the same amount of money that he started with after 200 plays. The results can vary, however. There are other outcomes that are not as likely to happen but are still possible. We can estimate the probability of each possible outcome by running 10,000 unique simulations of the coin toss game and observing the frequency of each outcome:

<figure class="fullwide">
  <img width="720" height="405" alt="Figure 1" src=".{{ page.id }}/fig-01.svg" />
</figure>

The chart above looks like it roughly approximates a binomial distribution, with outcomes closer to the breakeven amount more likely than outcomes further away. We can also separate the outcomes into categories based on whether the result is a profit, a loss, or a breakeven amount:

<figure class="equation">
  {% latex 02 %}
    \setlength{\arraycolsep}{1em}
    \begin{array}{@{\rule{0em}{1.25em}}|l|l|}
    \hline
    \text{Outcome} & \text{Probability}
    \\[0.25em]\hline
    Profit    & 0.477
    \\[0.25em]\hline
    Loss      & 0.465
    \\[0.25em]\hline
    Breakeven & 0.058
    \\[0.25em]\hline
    \end{array}
  {% endlatex %}
</figure>

While the simulated results might give us a pretty good approximation of the distribution of possible outcomes, there is a way to be more accurate.

## Computing the Distribution Analytically

Regardless of which betting strategy the gambler uses, the outcome of the repeated coin toss game depends on the total number of winning and losing plays. The order of winners and losers doesn't matter. For the fixed constant betting strategy, the following formula can be used to compute the final outcome based on the number of winning and losing games:

<figure class="equation">
  {% latex 03 %}
    V_n = W(+bV_0) + L(-bV_0)
  {% endlatex %}
</figure>

Since there are only two possible outcomes for each toss of the coin, the distribution of possible outcomes in a repeated coin toss game can be modeled as a binomial distribution. We can use the following probability mass function to compute the probability of each outcome based on the number of winning rounds relative to the total number of plays:

<figure class="equation">
  {% latex 04 %}
    \begin{aligned}
    & Pr(W = k) = \binom{n}{k} p^k (1 - p)^{n - k}
    \\[1em]
    &
    \begin{aligned}
    & n && \text{ is the total number of plays}
    \\
    & k && \text{ is the number of winning plays}
    \\
    & p && \text{ is the probability of winning a single round}
    \end{aligned}
    \end{aligned}
  {% endlatex %}
</figure>

While there are multiple techniques for computing the binomial coefficient, the formula using factorials seems to be the most common:

<figure class="equation">
  {% latex 05 %}
    \binom{n}{k} = \frac{n!}{k!(n - k)}
  {% endlatex %}
</figure>

However, I prefer to use the following alternative method when using a computer to perform the calculations:

<figure class="equation">
  {% latex 06 %}
    \binom{n}{k} =
    \begin{cases}
    \displaystyle \prod_{i = 1}^{k} \frac{n + 1 - i}{i} & \quad \text{if } k > 0
    \\[1.5em]
    1 & \quad \text{if } k = 0
    \end{cases}
  {% endlatex %}
</figure>

If the gambler plays 200 rounds of the coin toss game, there is a total of 201 unique possible outcomes. We can compute the probability of each outcome analytically by applying the formulas above. Using the same parameters of the coin toss game as before, the computed distribution looks like this:

<figure class="fullwide">
  <img width="720" height="405" alt="Figure 7" src=".{{ page.id }}/fig-07.svg" />
</figure>

This is the idealized form of the previous chart. Notice that the chart is symmetrical. The distribution of profitable outcomes to the right of $100 mirrors the distribution of losing outcomes to the left of $100. Grouping the results into profit, loss, and breakeven categories again, we get the following:

<figure class="equation">
  {% latex 08 %}
    \setlength{\arraycolsep}{1em}
    \begin{array}{@{\rule{0em}{1.25em}}|l|l|}
    \hline
    \text{Outcome} & \text{Probability}
    \\[0.25em]\hline
    Profit    & 0.472
    \\[0.25em]\hline
    Loss      & 0.472
    \\[0.25em]\hline
    Breakeven & 0.056
    \\[0.25em]\hline
    \end{array}
  {% endlatex %}
</figure>

While the symmetry between the distribution of winning and losing outcomes may not be surprising when using a fixed constant bet size, can we expect the same property hold true if we use the fixed fraction bet size strategy instead? Let's find out.

## Distributions on a Logarithmic Scale

To generate the equivalent probability distribution chart for the fixed fraction betting strategy, we need to compute the final outcomes differently. In this case, we use the following formula to determine the final outcome based on the number of winning and losing plays:

<figure class="equation">
  {% latex 09 %}
    V_n = V_0 \bigg[ (1 + b)^W (1 - b)^L \bigg]
  {% endlatex %}
</figure>

Plugging in the numbers, using the same parameters of the coin toss game as we used in previous examples, we get a probability distribution chart that looks completely different than the one above:

<figure class="fullwide">
  <img width="720" height="405" alt="Figure 10" src=".{{ page.id }}/fig-10.svg" />
</figure>

In the fixed fraction case, the gambler's bankroll can never fall below zero. This is why the chart doesn't show the existence of any possible outcomes below zero. Notice that there is a cluster of outcomes between zero and the $100 breakeven amount, while outcomes above $100 are fewer and more spaced out. The set of possible outcomes are not evenly spaced. It might be more appropriate to plot this chart on a logarithmic scale:

<figure class="fullwide">
  <img width="720" height="405" alt="Figure 11" src=".{{ page.id }}/fig-11.svg" />
</figure>

The outcomes are evenly distributed on a logarithmic scale, but the most likely outcomes are shifted to the left of the $100 breakeven amount. After playing 200 rounds, the gambler is far more likely to take a loss than to go home with a profit:

<figure class="equation">
  {% latex 12 %}
    \setlength{\arraycolsep}{1em}
    \begin{array}{@{\rule{0em}{1.25em}}|l|l|}
    \hline
    \text{Outcome} & \text{Probability}
    \\[0.25em]\hline
    Profit    & 0.069
    \\[0.25em]\hline
    Loss      & 0.931
    \\[0.25em]\hline
    \end{array}
  {% endlatex %}
</figure>

The most likely outcomes, outcomes with 100 winning plays and 100 losing plays, result in the gambler's $100 bankroll being whittled down to less than $2 after playing 200 games. Using a fixed fraction betting strategy changes the dynamic of the game because the results compound in a multiplicative fashion instead of an additive fashion.

## Balancing the Reward Function

As with the previous post, we assume the gambler always bets on heads. Recall the reward function we've been using so far for the fixed fraction betting strategy:

<figure class="equation">
  {% latex 13 %}
    R(X) =
    \begin{cases}
    1 + b & \quad \text{if } X = H
    \\
    1 - b & \quad \text{if } X = T
    \end{cases}
  {% endlatex %}
</figure>

With this reward function, for any given round of the coin toss game, the value of the winning amount is always equal to the amount the gambler risks on a loss. While this might seem like an equal tradeoff on the surface, we have shown above that a repeated coin toss game with this strategy results in a high probability of a losing outcome.

How can the reward function for the fixed fraction betting strategy be modified to give a balanced distribution of winning and losing outcomes? Instead of having a reward function in which the winning and losing amounts are the same, we need to come up with a reward function in which the multiplier applied to the gambler's bankroll for a winning play has the same magnitude as the multiplier used for a losing play. But the multipliers must have an equal magnitude on a logarithmic scale instead of a linear scale. Holding the gambler's risk of loss constant, we can define the reward function for the winning case in terms of the reward function used for the losing case using the following equation:

<figure class="equation">
  {% latex 14 %}
    \begin{aligned}
    \log{R(H)} &= -{\log{R(T)}}
    \\[1em]
               &= -{\log{(1 - b)}}
    \\[1em]
               &= \phantom{-}{\log{(1 - b)^{-1}}}
    \end{aligned}
  {% endlatex %}
</figure>

Taking the exponent of both sides, we can get the reward function for the winning case. Putting both the winning and losing reward functions together, we now have a balanced reward function that looks like this:

<figure class="equation">
  {% latex 15 %}
    R(X) =
    \begin{cases}
    (1 - b)^{-1}     & \quad \text{if } X = H
    \\
    \phantom{(}1 - b & \quad \text{if } X = T
    \end{cases}
  {% endlatex %}
</figure>

The reward multiplier that gets applied to the gambler's bankroll when the coin lands on heads is the multiplicative inverse of the multiplier used when the coin lands on tails. With the balanced reward function, the formula to compute the final outcome based on the number of winning and losing plays becomes:

<figure class="equation">
  {% latex 16 %}
    V_n = V_0 \bigg[ (1 - b)^{L - W} \bigg]
  {% endlatex %}
</figure>

The probability distribution chart now looks like this:

<figure class="fullwide">
  <img width="720" height="405" alt="Figure 17" src=".{{ page.id }}/fig-17.svg" />
</figure>

Of course, it makes more sense to plot this on a logarithmic chart:

<figure class="fullwide">
  <img width="720" height="405" alt="Figure 18" src=".{{ page.id }}/fig-18.svg" />
</figure>

With the modified reward function for the fixed fraction betting strategy, the gambler now has an equal probability of getting a winning outcome as he does a losing outcome. The breakdown of profit, loss, and breakeven outcomes is now the same as that of the fixed constant betting strategy:

<figure class="equation">
  {% latex 19 %}
    \setlength{\arraycolsep}{1em}
    \begin{array}{@{\rule{0em}{1.25em}}|l|l|}
    \hline
    \text{Outcome} & \text{Probability}
    \\[0.25em]\hline
    Profit    & 0.472
    \\[0.25em]\hline
    Loss      & 0.472
    \\[0.25em]\hline
    Breakeven & 0.056
    \\[0.25em]\hline
    \end{array}
  {% endlatex %}
</figure>

But while the distribution is balanced in terms of winning and losing outcomes, the payoff for a winning outcome always outweighs the payoff for a losing outcome---sometimes even by several orders of magnitude. I imagine an advantage player could probably find a way to profit from such a game.
