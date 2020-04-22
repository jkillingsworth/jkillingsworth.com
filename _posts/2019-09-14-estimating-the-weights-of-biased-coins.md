---
layout: post
title: Estimating the Weights of Biased Coins
---

Suppose we flip a coin four times. If the coin lands on heads, we win a dollar. If the coin lands on tails, we lose a dollar. After four tosses of the coin, the best possible outcome is a winning total of four dollars. The worst possible outcome is a loss of four dollars. Let's assume the coin is a biased coin. Furthermore, let's also assume a different biased coin is used on each flip depending on the total amount won or lost since the beginning of the game. How can we determine the bias of each coin given a probability mass function of the expected outcome?

<!--excerpt-->

In this post, I want to explore a technique that can be used to answer this question. I am hoping this model can be expanded to give some better insights into observations made in my previous posts regarding the [distribution of price fluctuations]({% post_url 2019-01-26-the-distribution-of-price-fluctuations %}) and the [strange behavior of the Chinese yuan]({% post_url 2019-02-10-the-very-strange-chinese-yuan %}).

## Markov Model

The aforementioned coin toss game can be modeled as a Markov chain. The game starts in the initial zero state. Each toss of the coin determines whether the system transitions to a higher state or a lower state. If the coin lands on heads, we move to a higher state. If the coin lands on tails, we move to a lower state. Each state determines the weight of the biased coin used in the next toss. Here is a graphical representation of the Markov model:

{% latex fig-01 %}
    \usepackage{tikz}
    \usetikzlibrary{automata,arrows}
    \begin{document}
    \begin{tikzpicture}[>=stealth',shorten >=1pt,auto,node distance=1.125in]
    \tikzset{every state/.style={minimum size=0.5in}}
    \node[state,initial right] (00)               {$S_0$};
    \node[state]               (+1) [above of=00] {$S_{+1}$};
    \node[state]               (+2) [right of=+1] {$S_{+2}$};
    \node[state]               (+3) [right of=+2] {$S_{+3}$};
    \node[state]               (+4) [right of=+3] {$S_{+4}$};
    \node[state]               (-1) [below of=00] {$S_{-1}$};
    \node[state]               (-2) [right of=-1] {$S_{-2}$};
    \node[state]               (-3) [right of=-2] {$S_{-3}$};
    \node[state]               (-4) [right of=-3] {$S_{-4}$};
    \path[->]
    (00) edge              node [swap] {$  p_0$} (+1)
    (+1) edge [bend right] node [swap] {$1-p_1$} (00)
    (+1) edge [bend left]  node        {$  p_1$} (+2)
    (+2) edge [bend left]  node        {$1-p_2$} (+1)
    (+2) edge [bend left]  node        {$  p_2$} (+3)
    (+3) edge [bend left]  node        {$1-p_3$} (+2)
    (+3) edge              node        {$  p_3$} (+4)
    (00) edge              node        {$  p_0$} (-1)
    (-1) edge [bend left]  node        {$1-p_1$} (00)
    (-1) edge [bend left]  node        {$  p_1$} (-2)
    (-2) edge [bend left]  node        {$1-p_2$} (-1)
    (-2) edge [bend left]  node        {$  p_2$} (-3)
    (-3) edge [bend left]  node        {$1-p_3$} (-2)
    (-3) edge              node        {$  p_3$} (-4);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

In this model, the coin in the zero state is always a fair coin. A fair coin will land on heads 50% of the time and tails 50% of the time. Additionally, this model is also symmetrical. The biased coins in the negative states are weighted inversely to the biased coins in the corresponding positive states. The diagram above can alternately be depicted as follows:

{% latex fig-02 %}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|>{$}wl{4.5em}<{$}|>{$}wl{9.25em}<{$}|>{$}wl{9.25em}<{$}|}
    \hline
    \text{State} & \text{Probability of Heads} & \text{Probability of Tails}
    \\[0.25em]\hline
    +3 & p_3 & 1 - p_3
    \\[0.25em]\hline
    +2 & p_2 & 1 - p_2
    \\[0.25em]\hline
    +1 & p_1 & 1 - p_1
    \\[0.25em]\hline
    \phantom{+}0 \text{ (start)} & p_0 = 0.5 & p_0 = 0.5
    \\[0.25em]\hline
    -1 & 1 - p_1 & p_1
    \\[0.25em]\hline
    -2 & 1 - p_2 & p_2
    \\[0.25em]\hline
    -3 & 1 - p_3 & p_3
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

Note that the positive and negative fourth states can only be terminal states, so there is never a transition out of them. Also keep in mind that the Markov model does not necessarily have to be symmetrical. I chose to make it symmetrical to reduce the number of variables and simplify the model.

## Monte Carlo Simulation

If we know the weights of the biased coins, we can use a Monte Carlo simulation to estimate the expected outcome of the coin toss game. Simply put, we can run a million simulated rounds of the coin toss game, with four flips in each round, and observe the distribution of possible outcomes. Suppose each coin is weighted fairly:

{% latex fig-03 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_0 & = 0.5
    \\
    p_1 & = 0.5
    \\
    p_2 & = 0.5
    \\
    p_3 & = 0.5
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The values above can be presented visually on a chart like this:

{% chart fig-04-binomial-equal-biases.svg %}

In the Monte Carlo simulation, we use a random number generator to simulate the outcome of each coin toss. We record the terminal state after each round of four coin tosses. After running one million rounds of the coin toss game, the distribution of terminal states looks like this:

{% chart fig-05-binomial-equal-pmfunc-simulated.svg %}

This gives us an estimate of the expected outcome after four tosses of the coin. Notice that only even terminal states are possible when there is an even number of coin tosses. Odd numbered terminal states would only be possible if there were an odd number of tosses. It is also worth noting that the chart above closely approximates a binomial distribution. Here is a breakdown of the expectation for each individual coin toss sequence:

{% chart fig-06-binomial-equal-tosses-simulated.svg %}

The values are all roughly the same, indicating that all possible coin toss sequences have about an equal probability of being observed in any given round of the coin toss game, assuming we're always using fair coins. The Monte Carlo simulation is a brute force approach that yields fairly accurate results. We can arrive at more precise values by solving analytically.

## Analytical Evaluation

With four tosses of the coin, there are sixteen possible coin toss sequences and five unique terminal states. The following table lists every possible coin toss sequence along with its terminal state and a formula for the probability of each combination:

{% latex fig-07 %}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|>{$}wl{4em}<{$}|>{$}wl{7em}<{$}|>{$}wl{12em}<{$}|}
    \hline
    \text{Sequence} & \text{Terminal State} & \text{Probability}
    \\[0.25em]\hline
    \texttt{TTTT} & -4 & p_0 \, p_1 \, p_2 \, p_3
    \\[0.25em]\hline
    \texttt{TTTH} & -2 & p_0 \, p_1 \, p_2 \, (1 - p_3)
    \\[0.25em]\hline
    \texttt{TTHT} & -2 & p_0 \, p_1 \, (1 - p_2) \, p_1
    \\[0.25em]\hline
    \texttt{THTT} & -2 & p_0 \, (1 - p_1) \, p_0 \, p_1
    \\[0.25em]\hline
    \texttt{HTTT} & -2 & p_0 \, (1 - p_1) \, p_0 \, p_1
    \\[0.25em]\hline
    \texttt{TTHH} & \phantom{+}0 & p_0 \, p_1 \, (1 - p_2) \, (1 - p_1)
    \\[0.25em]\hline
    \texttt{THTH} & \phantom{+}0 & p_0 \, (1 - p_1) \, p_0 \, (1 - p_1)
    \\[0.25em]\hline
    \texttt{HTTH} & \phantom{+}0 & p_0 \, (1 - p_1) \, p_0 \, (1 - p_1)
    \\[0.25em]\hline
    \texttt{THHT} & \phantom{+}0 & p_0 \, (1 - p_1) \, p_0 \, (1 - p_1)
    \\[0.25em]\hline
    \texttt{HTHT} & \phantom{+}0 & p_0 \, (1 - p_1) \, p_0 \, (1 - p_1)
    \\[0.25em]\hline
    \texttt{HHTT} & \phantom{+}0 & p_0 \, p_1 \, (1 - p_2) \, (1 - p_1)
    \\[0.25em]\hline
    \texttt{THHH} & +2 & p_0 \, (1 - p_1) \, p_0 \, p_1
    \\[0.25em]\hline
    \texttt{HTHH} & +2 & p_0 \, (1 - p_1) \, p_0 \, p_1
    \\[0.25em]\hline
    \texttt{HHTH} & +2 & p_0 \, p_1 \, (1 - p_2) \, p_1
    \\[0.25em]\hline
    \texttt{HHHT} & +2 & p_0 \, p_1 \, p_2 \, (1 - p_3)
    \\[0.25em]\hline
    \texttt{HHHH} & +4 & p_0 \, p_1 \, p_2 \, p_3
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

Keep in mind that the model is symmetrical. Let's use the following notation to represent probability of landing on each of the five possible terminal states:

{% latex fig-08 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = Pr(S_{0})
    \\[1em]
    r_2 & = Pr(S_{-2}) = Pr(S_{+2})
    \\[1em]
    r_4 & = Pr(S_{-4}) = Pr(S_{+4})
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Since the model is symmetrical, the chance of ending up on any given positive terminal state is equal to the probability of ending up on the corresponding negative terminal state. The equations above can be rewritten as follows:

{% latex fig-09 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = Pr(\texttt{TTHH}) + Pr(\texttt{THTH}) + Pr(\texttt{HTTH})
          + Pr(\texttt{THHT}) + Pr(\texttt{HTHT}) + Pr(\texttt{HHTT})
    \\[1em]
    r_2 & = Pr(\texttt{TTTH}) + Pr(\texttt{TTHT}) + Pr(\texttt{THTT}) + Pr(\texttt{HTTT})
    \\
        & = Pr(\texttt{THHH}) + Pr(\texttt{HTHH}) + Pr(\texttt{HHTH}) + Pr(\texttt{HHHT})
    \\[1em]
    r_4 & = Pr(\texttt{TTTT})
    \\
        & = Pr(\texttt{HHHH})
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Plugging in the probability formula for each of the coin toss combinations, the equations above can be expressed like this:

{% latex fig-10 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = 4\,\Big[\, p_0 \, (1 - p_1) \, p_0 \, (1 - p_1) \,\Big]
          + 2\,\Big[\, p_0 \, p_1 \, (1 - p_2) \, (1 - p_1) \,\Big]
    \\[1em]
    r_2 & = 2\,\Big[\, p_0 \, (1 - p_1) \, p_0 \, p_1 \,\Big]
          +    \Big[\, p_0 \, p_1 \, (1 - p_2) \, p_1 \,\Big]
          +    \Big[\, p_0 \, p_1 \, p_2 \, (1 - p_3) \,\Big]
    \\[1em]
    r_4 & = \phantom{\Big[}\, p_0 \, p_1 \, p_2 \, p_3 \,\phantom{\Big]}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

By definition, since our model is a symmetrical one, we know that the coin used in the zero state is always a fair coin:

{% latex fig-11 %}
    \begin{document}
    \begin{displaymath}
    p_0 = 0.5
    \end{displaymath}
    \end{document}
{% endlatex %}

Substituting the value above for the weight of the coin at the zero state and then simplifying, we can arrive at the following set of equations:

{% latex fig-12 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = \Big( 1 - p_1 \Big) \Big( 1 - p_1\,p_2 \Big)
    \\[1em]
    r_2 & = 0.5\,\Big( p_1 \,+\, p_1\,p_2 \,-\, p_1^2\,p_2 \,-\, p_1\,p_2\,p_3 \Big)
    \\[1em]
    r_4 & = 0.5\,\Big( p_1\,p_2\, p_3 \Big)
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

This set of equations represents the relationship between the weights of the biased coins and the expected outcome of the coin toss game. If we know the weights of the coins, we can compute the probability mass function of the expected outcome after four tosses of the coin. To illustrate, let's assume each coin is weighted fairly:

{% latex fig-13 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = 0.5
    \\
    p_2 & = 0.5
    \\
    p_3 & = 0.5
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

This is the same scenario we started with when doing the Monte Carlo simulation above, so we can expect to get the same or similar results. Plugging in the numbers, here are the computed results:

{% latex fig-14 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = 0.3750
    \\
    r_2 & = 0.2500
    \\
    r_4 & = 0.0625
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

These results can be presented visually with the following chart:

{% chart fig-15-binomial-equal-pmfunc-evaluated.svg %}

These are the exact figures that were estimated by the Monte Carlo simulation demonstrated in the previous section. This is a binomial distribution. The computed expectation of each individual coin toss sequence is show below:

{% chart fig-16-binomial-equal-tosses-evaluated.svg %}

This confirms the result estimated by the Monte Carlo simulation as well. When always using a fair coin, each of the sixteen combinations of coin toss sequences has an equal probability of being observed.

## Many Possible Solutions

We know from the previous sections that if all the coins are fairly weighted, then the expected outcome after four coin tosses is a binomial distribution. But what if we start with a binomial distribution as the expected outcome and work backwards to find the weights of the coins? We already know that having fairly weighted coins is one possible solution. But can there be other possible solutions as well?

Let's introduce the following notation. We have seen a graphical illustration of what a binomial distribution looks like in the previous section. Here is the symbolic form of the probability mass function for a binomial distribution:

{% latex fig-17 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    Pr(X = k) & = \frac{f(k)}{\sum_{i = 0}^{n} f(i)}, \quad k = 0, 1, 2, ..., n
    \\[1em]
    f(k)      & = \binom{n}{k} = \frac{n!}{k!(n - k)!}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

For succinctness, I am using an alternate zero based index to represent the terminal states. The following relationship holds:

{% latex fig-18 %}
    \begin{document}
    \begin{displaymath}
     Pr(S_t) = Pr(X = k), \quad t = 2k - n
    \end{displaymath}
    \end{document}
{% endlatex %}

Since there are only four coin tosses in the model we're using here, we can substitute:

{% latex fig-19 %}
    \begin{document}
    \begin{displaymath}
    n = 4
    \end{displaymath}
    \end{document}
{% endlatex %}

Now recall the following from the previous section:

{% latex fig-20 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = Pr(S_{0})
    \\[1em]
    r_2 & = Pr(S_{-2}) = Pr(S_{+2})
    \\[1em]
    r_4 & = Pr(S_{-4}) = Pr(S_{+4})
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can substitute the alternate notation and rewrite it like this:

{% latex fig-21 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = Pr(X = 2)
    \\[1em]
    r_2 & = Pr(X = 1) = Pr(X = 3)
    \\[1em]
    r_4 & = Pr(X = 0) = Pr(X = 4)
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Plugging in the numbers and doing the math, we can compute the following values to represent the binomial distribution:

{% latex fig-22 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = 0.3750
    \\
    r_2 & = 0.2500
    \\
    r_4 & = 0.0625
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

This represents the expected outcome after four coin tosses. Now we want to figure out what weights of the biased coins will give us this expectation. Based on the analysis in the previous section, we already know that playing the coin toss game with fairly weighted coins will give us our desired result:

{% latex fig-23 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = 0.5
    \\
    p_2 & = 0.5
    \\
    p_3 & = 0.5
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

However, this is not the only solution. As it turns out, there is an entire range of possible solutions that result in an expected outcome that takes the form of a binomial distribution. Consider the following equations derived in the previous section:

{% latex fig-24 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = \Big( 1 - p_1 \Big) \Big( 1 - p_1\,p_2 \Big)
    \\[1em]
    r_2 & = 0.5\,\Big( p_1 \,+\, p_1\,p_2 \,-\, p_1^2\,p_2 \,-\, p_1\,p_2\,p_3 \Big)
    \\[1em]
    r_4 & = 0.5\,\Big( p_1\,p_2\, p_3 \Big)
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Any solution that fits these equations is a valid solution provided that the following constraints are met:

{% latex fig-25 %}
    \begin{document}
    \begin{displaymath}
    0 \leq p_i \leq 1, \quad \forall i \in \{\, 1, 2, \dots, (n - 1) \,\}
    \end{displaymath}
    \end{document}
{% endlatex %}

The constraints are necessary because a coin cannot land on heads less than 0% of the time, nor can it land on heads more than 100% of the time. Using the equations above, we can rearrange them and solve for the weights of the biased coins in the +2 and +3 states in terms of the weight of the coin in the +1 state:

{% latex fig-26 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_2 & = \frac{1 - p_1 - r_0}{p_1 \, (1 - p_1)}
    \\[1em]
    p_3 & = \frac{2 r_4 \, (1 - p_1)}{1 - p_1 - r_0}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Any value for the weight of the biased coin in the +1 state can be used provided that the aforementioned constraints are met for the weights of all coins. There is a range of possible values for the coin in the +1 state. This range has a lower and upper bound. We can determine the lower bound for the weight of the coin in the +1 state by setting the weight of the coin in the +2 state to the maximum value of one:

{% latex fig-27 %}
    \begin{document}
    \begin{displaymath}
    p_2  = 1
    \end{displaymath}
    \end{document}
{% endlatex %}

With the weights of the coin in the +2 state held constant at one, we can solve for the weights of the coins in the +1 and + 3 states:

{% latex fig-28 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = 1 - \sqrt{r_0}
    \\[1em]
    p_3 & = \frac{2 r_4}{1 - \sqrt{r_0}}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Plugging the values into the equations:

{% latex fig-29 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = 0.3876
    \\
    p_2 & = 1.0000
    \\
    p_3 & = 0.3225
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here are the weights of all the coins when the weight of the coin in the +1 state is at the lower bound of the range:

{% chart fig-30-binomial-lower-biases.svg %}

Using the above weights, we can evaluate the expected outcome of the coin toss game and show that it indeed does result in a binomial distribution:

{% chart fig-31-binomial-lower-pmfunc-evaluated.svg %}

This is the same expected outcome we would get if all the coins were fair coins. However, consider the expectation of each individual coin toss sequence:

{% chart fig-32-binomial-lower-tosses-evaluated.svg %}

Not all coin toss combinations are equal. Some have a higher probability of being observed than others when the biased coins are weighted unfairly. We can observe a similar phenomenon when the weights of the coins are at the opposite extreme of the range of possible values. Suppose the weight of the coin in the +3 state is set to the maximum value of one:

{% latex fig-33 %}
    \begin{document}
    \begin{displaymath}
    p_3  = 1
    \end{displaymath}
    \end{document}
{% endlatex %}

With this held constant, we can find the upper bound for the weight of the coin in the +1 state and the lowest value for the weight of the coin in the +2 state:

{% latex fig-34 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = \frac{2 r_4 - 1 + r_0}{2 r_4 - 1}
    \\[1em]
    p_2 & = \frac{2 r_4 \, (2 r_4 - 1)}{2 r_4 - 1 + r_0}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Plugging the values into the equations:

{% latex fig-35 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = 0.5714
    \\
    p_2 & = 0.2188
    \\
    p_3 & = 1.0000
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here are the weights of all the coins when the weight of the coin in the +1 state is at the upper bound of the range:

{% chart fig-36-binomial-upper-biases.svg %}

Again, using the above weights, we can evaluate the expected outcome of the coin toss game and show that it indeed does result in a binomial distribution:

{% chart fig-37-binomial-upper-pmfunc-evaluated.svg %}

Like before, this is the same expected outcome we would get if all the coins were fair coins. But in this case, we get a different set of expectations for each coin toss sequence:

{% chart fig-38-binomial-upper-tosses-evaluated.svg %}

The examples illustrated in this section demonstrate two extremes of a range of possible values for the weights of biased coins. In both examples, these values yield an expected outcome in the shape of a binomial distribution when playing the coin toss game with four tosses. And this is the same outcome we can expect when playing the game with fair coins. Of course, there are many other possible values that will give us a binomial distribution as well.

## Hill Climbing Algorithm

What if the expected outcome is not a binomial distribution? Suppose we observe many rounds of the coin toss game and determine the expected outcome is a triangle distribution instead. We know the coins are biased, but how can we estimate the weights of the biased coins? Consider the symbolic form of a triangle distribution:

{% latex fig-39 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    Pr(X = k) & = \frac{f(k)}{\sum_{i = 0}^{n} f(i)}, \quad k = 0, 1, 2, ..., n
    \\[1em]
    f(k)      & = n + 2 - |2k - n|
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Using the techniques described in the previous section, we can compute the following values for the triangle distribution:

{% latex fig-40 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = 0.3333
    \\
    r_2 & = 0.2222
    \\
    r_4 & = 0.1111
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can also represent this visually with the following chart:

{% chart fig-41-triangle-learn-pmfunc.svg %}

And using the techniques described in the previous section, we can also compute the lower and upper bounds of the range of possible values that will yield a triangle distribution. The values are illustrated below:

{% chart fig-42-triangle-learn-biases-lower.svg %}
{% chart fig-43-triangle-learn-biases-upper.svg %}

While the weights illustrated above are valid to produce a triangle distribution, they might not be the best estimates to model a real world scenario. A coin that always lands on heads or always lands on tails, for example, might not be the most accurate model of reality. It might be better to come up with an intermediate solution that lies somewhere in between the two extremes.

Instead of trying to find an intermediate solution analytically, we can try to find one incrementally. We can start with an initial guess somewhere in the middle and then incrementally try to improve our initial estimate. We can repeat this process until we converge on a valid solution that fits the triangle distribution. This would effectively be a type of hill climbing algorithm.

Suppose we start with an initial guess of all fairly weighted coins. We can randomly pick one of the estimated weights, adjust it up or down by a small amount, and then measure whether or not the adjustment brings the computed outcome closer to that of a triangle distribution. If the adjustment is an improvement, then we accept it. If the adjustment is not an improvement, then we reject it and revert back to the previous value. We can repeat this process many times. Here is an illustration of this algorithm:

{% latex fig-44 %}
    \usepackage{tikz}
    \usetikzlibrary{shapes,arrows,positioning}
    \begin{document}
    \begin{tikzpicture}[>=stealth',shorten >=1pt,auto,minimum height=0.375in,inner sep=0.125in]
    \tikzstyle{terminal} = [draw,text width=1.0in,rectangle,rounded corners=0.1875in,align=center]
    \tikzstyle{process}  = [draw,text width=1.0in,rectangle]
    \tikzstyle{decision} = [draw,text width=0.5in,diamond,align=center]
    \node[terminal] (start)
    {
        Start
    };
    \node[process]  (step0) [below=0.25in of start]
    {
        Target values
        \\[1em]
        $
        \begin{aligned}
        r_0 & = 0.3333
        \\
        r_2 & = 0.2222
        \\
        r_4 & = 0.1111
        \end{aligned}
        $
    };
    \node[process]  (step1) [below=0.25in of step0]
    {
        Initial guess
        \\[1em]
        $
        \begin{aligned}
        p_1 & \gets 0.5
        \\
        p_2 & \gets 0.5
        \\
        p_3 & \gets 0.5
        \end{aligned}
        $
    };
    \node[process]  (step2) [below=0.25in of step1]
    {
        Iteration count
        \\[1em]
        $
        \begin{aligned}
        c & \gets 1000000
        \end{aligned}
        $
    };
    \node[process]  (step3) [below=0.25in of step2,text width=3in]
    {
        Random selection
        \\[1em]
        $
        \begin{aligned}
        s & = \text{random selection from}\, \{\, -0.00001, +0.00001\, \}
        \\
        i & = \text{random selection from}\, \{\, 1, 2, 3\, \}
        \\
        j & = \text{random selection from}\, \{\, 0, 2, 4\, \}
        \end{aligned}
        $
    };
    \node[process]  (step4) [below=0.25in of step3,text width=3in]
    {
        Measure proposed adjustment
        \\[1em]
        $
        \begin{aligned}
        d_{\mathrlap{\text{original}}\phantom{\text{proposed}}}
        & =
        r_j - (\text{calculate } r_j \text{ given } p_i)
        \\
        d_{\mathrlap{\text{proposed}}\phantom{\text{proposed}}}
        & =
        r_j - (\text{calculate } r_j \text{ given } p_i + s)
        \\
        v_{\text{improved}}
        & =
        |d_{\text{proposed}}| < |d_{\text{original}}|
        \end{aligned}
        $
    };
    \node[decision] (step5) [below=0.25in of step4]
    {
        $v_{\text{improved}}$
    };
    \node[process]  (step6) [below=0.25in of step5]
    {
        Accept value
        \\[1em]
        $
        \begin{aligned}
        p_i & \gets p_i + s
        \end{aligned}
        $
    };
    \node[process]  (step7) [below=0.25in of step6]
    {
        Iteration count
        \\[1em]
        $
        \begin{aligned}
        c & \gets c - 1
        \end{aligned}
        $
    };
    \node[decision] (step8) [below=0.25in of step7]
    {
        $c > 0$
    };
    \node[terminal] (end)   [below=0.25in of step8]
    {
        End
    };
    \draw[->]
    (start) edge              (step0)
    (step0) edge              (step1)
    (step1) edge              (step2)
    (step2) edge              (step3)
    (step3) edge              (step4)
    (step4) edge              (step5)
    (step5) edge node {true}  (step6)
    (step6) edge              (step7)
    (step7) edge              (step8)
    (step8) edge node {false} (end);
    \draw[->] (step5.west) node [above left]  {false} -- ++(-0.5in,0in) |- (step7.west);
    \draw[->] (step8.east) node [above right] {true}  -- ++(+1.5in,0in) |- (step3.east);
    \end{tikzpicture}
    \end{document}
{% endlatex %}

This algorithm iterates one million times, accepting or rejecting a proposed increment to one of the estimated weights. It's basically a trial and error process. Applying this algorithm gives us the following estimates for the weights of the biased coins:

{% chart fig-45-triangle-learn-biases-estimated.svg %}

Playing the coin toss game with the above weights for the biased coins will yield an expected outcome that is very close to the triangle distribution. This is not an exact solution, but it is very close. This is also not the only solution. One of the idiosyncrasies of this approach is that the result can be different depending on what values are used for the initial guess. Remember, there is a range of possible solutions, all equally valid.

The following sections catalog the application of this estimation process to a few variations of a discrete double exponential distribution instead of a triangle distribution.

## Exponential Distribution, Parameter = 0.5

Suppose we model the expected outcome as a discrete double exponential distribution such that the expectation of the game finishing on a given terminal state is 0.5 times that of the terminal state with the next highest expectation. Here is the probability mass function:

{% latex fig-46 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    Pr(X = k) & = \frac{f(k)}{\sum_{i = 0}^{n} f(i)}, \quad k = 0, 1, 2, ..., n
    \\[1em]
    f(k)      & = 0.5^{a}, \quad a = \frac{|2k - n| - n}{2}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is a graphical representation of the probability mass function above:

{% chart fig-47-exponential-05-pmfunc.svg %}

The following charts are the lower and upper bounds of the range of possible values for weights that result in the expected outcome defined above:

{% chart fig-48-exponential-05-biases-lower.svg %}
{% chart fig-49-exponential-05-biases-upper.svg %}

Here are the estimated weights using the hill climbing algorithm:

{% chart fig-50-exponential-05-biases-estimated.svg %}

The above values were estimated starting with an initial guess of fairly weighted coins and running the hill climbing algorithm through one million iterations.

## Exponential Distribution, Parameter = 0.4

Suppose we model the expected outcome as a discrete double exponential distribution such that the expectation of the game finishing on a given terminal state is 0.4 times that of the terminal state with the next highest expectation. Here is the probability mass function:

{% latex fig-51 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    Pr(X = k) & = \frac{f(k)}{\sum_{i = 0}^{n} f(i)}, \quad k = 0, 1, 2, ..., n
    \\[1em]
    f(k)      & = 0.4^{a}, \quad a = \frac{|2k - n| - n}{2}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is a graphical representation of the probability mass function above:

{% chart fig-52-exponential-04-pmfunc.svg %}

The following charts are the lower and upper bounds of the range of possible values for weights that result in the expected outcome defined above:

{% chart fig-53-exponential-04-biases-lower.svg %}
{% chart fig-54-exponential-04-biases-upper.svg %}

Here are the estimated weights using the hill climbing algorithm:

{% chart fig-55-exponential-04-biases-estimated.svg %}

The above values were estimated starting with an initial guess of fairly weighted coins and running the hill climbing algorithm through one million iterations.

## Exponential Distribution, Parameter = 0.3

Suppose we model the expected outcome as a discrete double exponential distribution such that the expectation of the game finishing on a given terminal state is 0.3 times that of the terminal state with the next highest expectation. Here is the probability mass function:

{% latex fig-56 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    Pr(X = k) & = \frac{f(k)}{\sum_{i = 0}^{n} f(i)}, \quad k = 0, 1, 2, ..., n
    \\[1em]
    f(k)      & = 0.3^{a}, \quad a = \frac{|2k - n| - n}{2}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is a graphical representation of the probability mass function above:

{% chart fig-57-exponential-03-pmfunc.svg %}

The following charts are the lower and upper bounds of the range of possible values for weights that result in the expected outcome defined above:

{% chart fig-58-exponential-03-biases-lower.svg %}
{% chart fig-59-exponential-03-biases-upper.svg %}

Here are the estimated weights using the hill climbing algorithm:

{% chart fig-60-exponential-03-biases-estimated.svg %}

The above values were estimated starting with an initial guess of fairly weighted coins and running the hill climbing algorithm through one million iterations.

## Next Steps

I want to explore this estimation technique in more depth. I would like to see what the estimated weights look like when the coin toss game is played with many coin tosses per round instead of just four. Perhaps this will be the topic of a later post.

Also, I would also like to come up with a more meaningful cost function for the hill climbing algorithm, one that would give preference to valid values closer to the middle of the range of possible values. With the implementation presented in this post, there is a plateau of valid values that can be estimated. The value that it converges to is highly dependent on the initial guess. Having a better cost function would allow the algorithm to converge to the same solution regardless of the starting point. I'm not sure yet how to do this.

{% accompanying_src_link %}
