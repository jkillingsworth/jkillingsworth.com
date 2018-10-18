---
layout: post
title: How Much Is an Option Worth?
---

Consider an at-the-money call option with a strike price of $50. The underlying asset is currently trading at $50 per share. Assume it's a European style option. One trader wants to take the long side of the contract. Another trader wants to take the short side. How can they agree on a fair price?

<!--excerpt-->

The intrinsic value of the option at the time of expiration is a function of two things: the strike price of the option contract and the market price of the underlying asset when the option expires. You can compute the value of the option at expiration using this equation:

{% latex 01 %}
    \begin{aligned}
    &
    \begin{aligned}
    & V(S_T) =
    \begin{cases}
    0       & \quad \text{if } S_T \leq K
    \\
    S_T - K & \quad \text{if } S_T > K
    \end{cases}
    \end{aligned}
    \\[1em]
    &
    \begin{aligned}
    & S_T && \text{is the price of the underlying at expiration}
    \\
    & K   && \text{is the strike price of the option}
    \end{aligned}
    \end{aligned}
{% endlatex %}

Since the market price of the underlying can fluctuate over time, the price of the underlying at the time of expiration cannot be known in advance. To value an option, you must somehow guess what the price of the underlying might be when the option expires.

## Averaging Predictions

Let's say the seller of the option contract predicts the price of the underlying will be trading at $40 per share at expiration. The buyer, on the other hand, speculates that the underlying will be trading at $60 per share at expiration. Using the formula above, we can compute the intrinsic value of the option at each of the anticipated outcomes:

{% latex 02 %}
    \setlength{\arraycolsep}{1em}
    \begin{array}{@{\rule{0em}{1.25em}}|l|l|}
    \hline
    \multicolumn{1}{ }{ } & \multicolumn{1}{l}{\phantom{Pr(S_T)}}
    \\[-1.25em]
    S_T & V(S_T)
    \\[0.25em]\hline
    \$40 & \$0
    \\[0.25em]\hline
    \$60 & \$10
    \\[0.25em]\hline
    \end{array}
{% endlatex %}

The seller reasons that offering the option at any price greater than zero would be to his advantage since, according to his prediction, the option will expire worthless. The buyer figures that purchasing the option at any price below $10 is an opportunity for profit. The two traders decide that the fairest price can be obtained by taking the average value of the option for the two predicted outcomes:

{% latex 03 %}
    \begin{aligned}
    \mathbb{E}[V(S_T)] & = \frac{V(S_T = \$40) + V(S_T = \$60)}{2}
    \\[1em]
                       & = \$5
    \end{aligned}
{% endlatex %}

In valuing the option, the two traders effectively give equal weight to the probability of each outcome occurring. The trader who predicts the correct outcome profits from the transaction, while the trader who predicts incorrectly takes a loss.

## Discrete Distributions

In a more realistic scenario, there might be many possible outcomes. Some outcomes might have a higher probability of occurring than others. Consider another example with the following set of possible outcomes:

{% latex 04 %}
    X = \{\ \$20,\ \$30,\ \$40,\ \$50,\ \$60\,\ \$70,\ \$80\ \}
{% endlatex %}

Using the formula above again, we can compute the intrinsic value of the option at each of the possible outcomes:

{% latex 05 %}
    \setlength{\arraycolsep}{1em}
    \begin{array}{@{\rule{0em}{1.25em}}|l|l|}
    \hline
    \multicolumn{1}{ }{ } & \multicolumn{1}{l}{\phantom{Pr(S_T)}}
    \\[-1.25em]
    S_T & V(S_T)
    \\[0.25em]\hline
    \$20 & \$0
    \\[0.25em]\hline
    \$30 & \$0
    \\[0.25em]\hline
    \$40 & \$0
    \\[0.25em]\hline
    \$50 & \$0
    \\[0.25em]\hline
    \$60 & \$10
    \\[0.25em]\hline
    \$70 & \$20
    \\[0.25em]\hline
    \$80 & \$30
    \\[0.25em]\hline
    \end{array}
{% endlatex %}

Now let's assume the probability of each outcome is this:

{% latex 06 %}
    \setlength{\arraycolsep}{1em}
    \begin{array}{@{\rule{0em}{1.25em}}|l|l|}
    \hline
    S_T & Pr(S_T)
    \\[0.25em]\hline
    \$20 & 0.05
    \\[0.25em]\hline
    \$30 & 0.10
    \\[0.25em]\hline
    \$40 & 0.15
    \\[0.25em]\hline
    \$50 & 0.40
    \\[0.25em]\hline
    \$60 & 0.15
    \\[0.25em]\hline
    \$70 & 0.10
    \\[0.25em]\hline
    \$80 & 0.05
    \\[0.25em]\hline
    \end{array}
{% endlatex %}

We can compute the expected value of the option as a weighted average:

{% latex 07 %}
    \begin{aligned}
    \mathbb{E}[V(S_T)] & = \sum_{x \in X}^{ } Pr(S_T = x)V(S_T = x)
    \\[1em]
                       & = \$5
    \end{aligned}
{% endlatex %}

This approach lets us to model a prediction as a probability mass function across a range of values. The prediction might be based on historical price action or it might simply be an arbitrary belief chosen at our discretion.

## Continuous Distributions

In real markets, prices don't always snap neatly to $10 increments. Tick sizes can be as small as a penny or even smaller. It might be better to model a price prediction as a continuous probability distribution. Let's consider a set of possible outcomes that can span across a continuous range of values:

{% latex 08 %}
    X = \{\ x \in \mathbb{R}\ |\ x \geq 0\ \}
{% endlatex %}

Prices can't fall below zero, so the range of possible values has a lower bound at zero. As before, some outcomes might have a higher probability of occurring than others. The cumulative distribution function is the probability that, when the option expires, the price of the underlying is less than or equal to a given value:

{% latex 09 %}
    F(x) = Pr(S_T \leq x)
{% endlatex %}

From probability theory, we know that the probability density function is just the derivative of the cumulative distribution function:

{% latex 10 %}
    f(x) = \frac{d}{dx} F(x)
{% endlatex %}

If we can come up with a probability density function that models our prediction, we can compute the expected value of the option by integrating over the range of possible outcomes:

{% latex 11 %}
    \mathbb{E}[V(S_T)] = \int_{0}^{\infty} V(x)f(x)\,dx
{% endlatex %}

Here the lower limit is zero because the price of the underlying can never fall below zero. The upper limit approaches infinity if the density function is unbounded. If using a bounded density function, you can set the limits accordingly. Let's look at a concrete example. Suppose we model our prediction as a simple triangular distribution:

{% latex 12 %}
    \begin{aligned}
    & f(x) =
    \begin{cases}
    \displaystyle \frac{(x - 20)}{900} & \quad \text{if } 20 \leq x < 50
    \\[1em]
    \displaystyle \frac{(80 - x)}{900} & \quad \text{if } 50 \leq x < 80
    \\[1em]
    0                                  & \quad \text{otherwise}
    \end{cases}
    \end{aligned}
{% endlatex %}

Since the probability density is zero for all values outside the range spanning from $20 to $80, we can ignore values outside of this range. Furthermore, since we're only considering at-the-money call options with a strike price of $50 in this context, we know the intrinsic value of the option upon expiration is always zero if the underlying is trading below $50. Plugging in the density function and simplifying:


{% latex 13 %}
    \begin{aligned}
    \mathbb{E}[V(S_T)] & = \int_{50}^{80} \frac{(x - 50)(80 - x)}{900}\,dx
    \\[1em]
                       & = \$5
    \end{aligned}
{% endlatex %}

Following this approach, you can plug in any probability distribution function that models the volatility of the underlying asset price.

## Other Considerations

The examples above present a few simple models for estimating the value of an option contract. In these examples, the value of an option depends largely on how wide or how narrow the price of the underlying is expected to fluctuate. However, there are other factors to take into consideration when estimating the value of an option contract. Here are a few of them:

* Choosing a probability distribution that best approximates price fluctuations
* Evaluating price changes on a logarithmic scale vs. a linear scale
* How the passage of time affects the value of an option
* Computing the value of American-style options on dividend paying stocks
* Heteroskedastic variances, price jumps, and other such hazards

There are other, more sophisticated option pricing models available which take these and other factors into consideration. I might explore some of these topics in future posts.
