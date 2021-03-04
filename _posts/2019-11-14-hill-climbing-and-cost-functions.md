---
layout: post
title: Hill Climbing and Cost Functions
---

If you're climbing a hill, you know you've reached the top when you can't take any further steps that lead to a higher elevation. But if the hill is actually a plateau with a flat top, the topmost point you reach can depend largely on where you started climbing. In this post, I elaborate on the topic of my previous post titled [*Estimating the Weights of Biased Coins*]({% post_url 2019-09-14-estimating-the-weights-of-biased-coins %}). This post presents the results of an improved hill climbing algorithm and also some ideas for ranking the different solutions that fall on a plateau of valid values.

<!--excerpt-->

## Hill Climbing Algorithm

The hill climbing algorithm used in my [previous post]({% post_url 2019-09-14-estimating-the-weights-of-biased-coins %}) is a stochastic hill climbing algorithm. It randomly chooses a small increment to one of the weights and then either accepts or rejects the move, depending on whether or not the adjustment brings the computed outcome closer to that of the target distribution.

Instead of randomly picking a single move, we can consider the set of all possible moves and pick the one with the steepest ascent up the hill. To do this, we can increment each one of the weights in both the positive and negative direction. If there are three weights, there are six possible moves. We can compute the value of a cost function for each move and then choose the best one. This process can be repeated until the none of the possible moves offers and improvement to the value of the cost function associated with the previous estimate. To illustrate, consider the following target distribution:

{% chart fig-01-target-pmfunc.svg %}

Suppose this represents the expected outcome of playing the weighted coin toss game with four tosses, as described in my [previous post]({% post_url 2019-09-14-estimating-the-weights-of-biased-coins %}). The target values are thus:

{% latex fig-02 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = 0.4000
    \\
    r_2 & = 0.2000
    \\
    r_4 & = 0.1000
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

We want to figure out what weights of the biased coins will give us an expected outcome that matches this target distribution. We start by making an initial guess. Suppose we start with an initial guess of all fairly weighted coins, as illustrated below:

{% chart fig-03-estimate-equal-05-begin-biases.svg %}

These weights can be represented using the following notation:

{% latex fig-04 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = 0.5000
    \\
    p_2 & = 0.5000
    \\
    p_3 & = 0.5000
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Given this initial guess, we can calculate what the expectation of the coin toss game would be if the coins were weighted according to our initial estimate. Recall the following equations derived in the [previous post]({% post_url 2019-09-14-estimating-the-weights-of-biased-coins %}):

{% latex fig-05 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = \Big( 1 - p_1 \Big) \Big( 1 - p_1\,p_2 \Big)
    \\[1em]
    r_2 & = 0.5\,\Big( p_1 \,+\, p_1\,p_2 \,-\, p_1^2\,p_2 \,-\, p_1\,p_2\,p_3 \Big)
    \\[1em]
    r_4 & = 0.5\,\Big( p_1\,p_2\,p_3 \Big)
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Plugging in the numbers and doing the math, here are the computed values based on the initial estimate of the weights:

{% latex fig-06 %}
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

This does not match our target values, so we know that our initial guess is not correct. But how incorrect is it? Can we quantify the fitness of our estimate? Indeed, we can use a cost function to determine how close our estimate is to the desired result. In this example, we use the sum of squared errors as our cost function:

{% latex fig-07 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    C & = \Big( 0.3750 - 0.4000 \Big)^2
        + \Big( 0.2500 - 0.2000 \Big)^2
        + \Big( 0.0625 - 0.1000 \Big)^2
    \\[1em]
      & = 0.00453125
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Our objective is to revise the estimate to minimize the value of the cost function. Lower values indicate a more favorable estimate. A zero value is the most ideal. Incrementing each one of the estimated weights by a small step size, in both the positive and negative direction, there are six possible revisions we can make to our initial estimate:

{% latex fig-08 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|>{$}wl{8em}<{$}|>{$}wl{6em}<{$}|}
    \hline
    \text{Proposed Revision} & \text{Cost}
    \\[0.25em]\hline
    \text{Set $p_1$ to $0.5001$} & 0.00453907
    \\[0.25em]\hline
    \text{Set $p_2$ to $0.5001$} & 0.00453165
    \\[0.25em]\hline
    \text{Set $p_3$ to $0.5001$} & 0.00452906
    \\[0.25em]\hline
    \text{Set $p_1$ to $0.4999$} & 0.00452345^{\,*}
    \\[0.25em]\hline
    \text{Set $p_2$ to $0.4999$} & 0.00453094
    \\[0.25em]\hline
    \text{Set $p_3$ to $0.4999$} & 0.00453344
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

The proposed revision with the lowest value for the cost function is the move with the steepest ascent up the hill. If we choose the proposed revision with the lowest value for the cost function, our revised estimate then becomes:

{% latex fig-09 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = 0.4999
    \\
    p_2 & = 0.5000
    \\
    p_3 & = 0.5000
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Using this revised estimate as our new baseline, we can repeat this process again and again until none of the proposed revisions offers an improvement to the value of the cost function associated with the previous estimate. After many iterations, we converge on the following values:

{% latex fig-10 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    p_1 & = 0.4741
    \\
    p_2 & = 0.5050
    \\
    p_3 & = 0.8354
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is a visual representation of the final result:

{% chart fig-11-estimate-equal-05-final-biases.svg %}

If we play the coin toss game with these weights, the expected outcome matches the target distribution. However, as demonstrated in a [previous study]({% post_url 2019-09-14-estimating-the-weights-of-biased-coins %}#exponential-distribution-parameter--05), this is not the only solution possible for the given target distribution.

## Different Starting Points

Instead of starting with an initial estimate of all fairly weighted coins, what would happen if we applied the hill climbing algorithm described above starting with a different set of weights for the initial guess? Recall the target distribution we are trying to find the weights for:

{% chart fig-12-target-pmfunc.svg %}

Now suppose we start with the following initial estimate:

{% chart fig-13-estimate-slope-02-begin-biases.svg %}

These are ascending weights instead of equal weights. If we revise the estimate repeatedly over many iterations, following the same steps described in the previous section, the estimate eventually converges on the following values:

{% chart fig-14-estimate-slope-02-final-biases.svg %}

As you can see, the final results found by the hill climbing algorithm depend on the initial estimate of the weights. Since there are multiple valid solutions, the hill we want to climb is actually a plateau that is flat at the top. To visualize, consider the following illustration:

{% chart fig-15-climb-hill-east.svg %}

A hiker wants to get to the top of a hill. If he starts at the base of the hill and always takes steps in the direction that leads to a higher elevation, he will eventually reach the top of the hill. Once at the top, the hiker cannot take any additional steps that would lead to a higher elevation. In the illustration above, the hiker starts on the east side of the hill. Look at what happens if the hiker starts on the west side of the hill:

{% chart fig-16-climb-hill-west.svg %}

Regardless of starting position, the hiker always winds up in the same position at the top of the hill. But this only happens with certain types of hills. Consider what happens if the hill is actually a plateau with a flat top:

{% chart fig-17-climb-plat-east.svg %}

When starting from the base on the east side of the plateau, the hiker reaches the topmost point located on the eastern rim. At this point, the hiker cannot take any further steps that would lead to a higher elevation. Now look what happens if the hiker starts at the base on the west side of the plateau: 

{% chart fig-18-climb-plat-west.svg %}

In this case, the topmost point reached by the hiker is located on the western rim of the plateau. When the hill is really a plateau, the hill climbing algorithm does not converge to the same solution for different starting positions. However, it might be possible to adapt the cost function in such a way that would give preference to some valid solutions over others. For example, a scoring function might be used to give preference to certain values based on proximity to the edge of the plateau.

## Scoring Values on the Plateau

Here I want to consider two different scoring functions that can be used to rank the values on a plateau of possible solutions for the weighted coin toss game. Each of these scoring functions could be used as a secondary cost function in cases where the primary cost function returns the most optimal value of zero.

The first scoring function I want to consider gives preference to weights that are nearest to the center of the range of possible weights:

{% latex fig-19 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{S_a}\phantom{S_b}
    =
    \Big( p_1 - 0.5 \Big)^2 + \Big( p_2 - 0.5 \Big)^2 + \Big( p_3 - 0.5 \Big)^2
    \end{displaymath}
    \end{document}
{% endlatex %}

The second scoring function I want to consider gives preference to weights that are nearest to their neighboring weights:

{% latex fig-20 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{S_b}\phantom{S_b}
    =
    \Big( p_1 - 0.5 \Big)^2 + \Big( p_2 - p_1 \Big)^2 + \Big( p_3 - p_2 \Big)^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Now let's consider the first set of weights we found for the target distribution after applying the hill climbing algorithm starting with an initial guess of all fairly weighted coins:

{% chart fig-21-estimate-equal-05-final-biases.svg %}

Plugging these weights into the scoring functions:

{% latex fig-22 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mathrlap{S_a}\phantom{S_b} & = 0.11318897
    \\
    \mathrlap{S_b}\phantom{S_b} & = 0.11078978
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Now let's consider the second set of weights we found for the target distribution after applying the hill climbing algorithm starting with an initial guess of ascending weights:

{% chart fig-23-estimate-slope-02-final-biases.svg %}

Plugging these weights into the scoring functions:

{% latex fig-24 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \mathrlap{S_a}\phantom{S_b} & = 0.07284686
    \\
    \mathrlap{S_b}\phantom{S_b} & = 0.07980389
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

While both sets of weights are valid solutions for the given target distribution, the scoring functions give preference to one over the other. In this case, both scoring functions give preference to the second set of weights. But these two sets of weights are just two possible solutions in an entire range of possible solutions in which the primary cost function evaluates to zero.

If we only consider the range of possible solutions in which the primary cost function evaluates to zero, which are the only valid solutions for the given target distribution, how do we go about finding the one with the most optimal score for each of the two scoring functions? We can start by plotting the scores across the range of possible values. Based on the results derived in the [previous post]({% post_url 2019-09-14-estimating-the-weights-of-biased-coins %}), the weights of the biased coins the +2 and +3 states can be stated in terms of the weight of the coin in the +1 state:

{% latex fig-25 %}
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

Additionally, the value of the weight of the biased coin in the +1 state is limited to a range with a lower and upper bound:

{% latex fig-26 %}
    \begin{document}
    \begin{displaymath}
    p_{1,\sscr{min}} \,\leq\, p_1 \,\leq\, p_{1,\sscr{max}}
    \end{displaymath}
    \end{document}
{% endlatex %}

The minimum value at the lower bound is:

{% latex fig-27 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{p_{1,\sscr{min}}}\phantom{p_{1,\sscr{max}}} = 1 - \sqrt{r_0}
    \end{displaymath}
    \end{document}
{% endlatex %}

The maximum value at the upper bound is:

{% latex fig-28 %}
    \begin{document}
    \begin{displaymath}
    \mathrlap{p_{1,\sscr{max}}}\phantom{p_{1,\sscr{max}}} = \frac{2 r_4 - 1 + r_0}{2 r_4 - 1}
    \end{displaymath}
    \end{document}
{% endlatex %}

From this, we can represent the range of all valid solutions for the target distribution on a single axis. We can plot the weights on the horizontal axis and the scores on the vertical axis. Recall the values of the target distribution presented earlier:

{% latex fig-29 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_0 & = 0.4000
    \\
    r_2 & = 0.2000
    \\
    r_4 & = 0.1000
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is the plot of the first scoring function for all weights in which the primary cost function evaluates to zero for the target distribution:

{% chart fig-30-optimize-score-A-scores.svg %}

The most optimal point can be found by taking the derivative of the scoring function and finding the root:

{% latex fig-31 %}
    \begin{document}
    \begin{displaymath}
    \frac{d\mathrlap{S_a}\phantom{S_b}}{dp_1} = 0, \quad p_1 \approx 0.4422
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is the most optimal set of weights determined by the scoring function:

{% chart fig-32-optimize-score-A-biases.svg %}

Here is the plot of the second scoring function for all weights in which the primary cost function evaluates to zero for the target distribution:

{% chart fig-33-optimize-score-B-scores.svg %}

The most optimal point can be found by taking the derivative of the scoring function and finding the root:

{% latex fig-34 %}
    \begin{document}
    \begin{displaymath}
    \frac{d\mathrlap{S_b}\phantom{S_b}}{dp_1} = 0, \quad p_1 \approx 0.4487
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is the most optimal set of weights determined by the scoring function:

{% chart fig-35-optimize-score-B-biases.svg %}

These two results are different than those found by the hill climbing algorithm. For both scoring functions above, I used a numerical root finding method to find the root of the derivative. I don't think a closed form solution is possible in either of these two cases. This approach to finding the optimal values may not work if the scoring function is not differentiable at all points that fall within the lower and upper bound.

## Next Steps

The variation of the hill climbing algorithm presented here converges to a solution very quickly in a finite number of iterations. However, it does not converge to a unique solution for all starting points. When using a scoring function like the ones presented above, a unique solution can be found for a problem that would otherwise have an infinite number of possible solutions.

My goal is to find a way to combine the cost function used in the hill climbing algorithm with a scoring function that gives curvature to an otherwise flat plateau. This would allow the hill climbing algorithm to converge to a unique solution regardless of the initial guess. The plateau essentially becomes a ridge. My initial attempts at combining the cost function with a scoring function have resulted in a method that seems to find its way to the ridge, but then gets stuck when trying to ascend the ridge.

Ultimately, I want to come up with a technique that finds a unique solution to the weighted coin toss game when there is a large number of coin tosses instead of just four. And I want to do so in a way that is computationally efficient. Variations of the hill climbing algorithm may or may not be the best approach. I plan to post more on this topic as I continue to explore new ideas.

{% accompanying_src_link %}
