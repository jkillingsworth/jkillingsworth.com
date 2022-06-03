---
layout: post
title: Minimizing with Gradient Descent
---

The [previous post]({% post_url 2020-08-16-visualizing-the-climb-up-the-hill %}) demonstrates the use of a hill climbing algorithm to find a set of parameters that minimize a cost function associated with a coin toss game. In this post, I want to explore the use of a gradient descent algorithm as an alternative. The two classes of algorithms are very similar in that they both iteratively update an estimated parameter set. But while the hill climbing algorithm only updates one parameter at a time, the gradient descent approach updates all parameters in proportion to the direction of steepest descent.

<!--excerpt-->

## The Algorithm

The gradient descent algorithm is a way of finding a local minimum of a differentiable function. In this case, we'll use the same cost function used in the previous post titled [*Visualizing the Climb up the Hill*]({% post_url 2020-08-16-visualizing-the-climb-up-the-hill %}) as our example. Recall the following equations based on the model of the coin toss game:

{% latex fig-01 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_1 & = 0.5 \1 \brace1(){ 1 - p_1 \0 p_2 }
    \\[1em]
    r_3 & = 0.5 \0 p_1 \0 p_2
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The two equations above must hold true if we have a valid set of weights for a given target distribution. If we estimate a set of weights that do not satisfy these equations, then we know the estimate can be improved. The sum of squared errors makes for a convenient cost function:

{% latex fig-02 %}
    \begin{document}
    \begin{displaymath}
    C(\mathbf{p})
    =
    \brace2[]{ r_1 - 0.5 \1 \brace1(){ 1 - p_1 \0 p_2 } }^2
    +
    \brace2[]{ r_3 - 0.5 \0 p_1 \0 p_2 }^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Note the use of a parameter vector for the input. As you might expect, this is a convenient notation for multivariable functions. Now once we have a cost function to work with, we also need to find the gradient of the cost function:

{% latex fig-03 %}
    \begin{document}
    \begin{displaymath}
    \nabla C(\mathbf{p})
    =
    \begin{matrix}{l}
    \dfrac{\pderiv C}{\pderiv p_1}
    \\[2em]
    \dfrac{\pderiv C}{\pderiv p_2}
    \end{matrix}
    \end{displaymath}
    \end{document}
{% endlatex %}

The gradient is a vector containing partial derivatives of the cost function, one for each of the variables. Each component is the slope of the function along the corresponding axis. The vector points in the direction of steepest ascent. The apply the gradient descent algorithm, we need to subtract a value in the direction of the gradient from an estimated value to obtain an improved value. Here is the formula:

{% latex fig-04 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{p}_{k + 1} = \mathbf{p}_{k} - \gamma_k \nabla C(\mathbf{p}_k)
    \end{displaymath}
    \end{document}
{% endlatex %}

Starting with an initial guess, we can apply the above repeatedly until the gradient has a magnitude of zero. Once the gradient is zero, we know we have reached a local minimum. In practice, however, you might want to stop once the gradient is sufficiently small. In this example, we terminate once the magnitude of the gradient is below a predetermined step size:

{% latex fig-05 %}
    \begin{document}
    \begin{displaymath}
    s = 0.0001
    \end{displaymath}
    \end{document}
{% endlatex %}

To ensure each increment adjusts the estimate by a fixed step size, we need to multiply the gradient by a scalar factor called the learning rate:

{% latex fig-06 %}
    \begin{document}
    \begin{displaymath}
    \gamma_k = \frac{s}{\| \nabla C(\mathbf{p}_k) \|}
    \end{displaymath}
    \end{document}
{% endlatex %}

In this instance, the learning rate is simply the step size divided by the magnitude of the gradient vector. The learning rate is recomputed each iteration.

## Visualizations

I want to compare and contrast the gradient descent algorithm outlined above with the hill climbing algorithms used in the previous post. To do so, let's use the same target distribution as we used before:

{% chart fig-07-target-pmfunc.svg %}

These values can be represented symbolically like this:

{% latex fig-08 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    r_1 & = 0.3333
    \\
    r_3 & = 0.1667
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

These values can be used to produce a concrete form of the cost function:

{% latex fig-09 %}
    \begin{document}
    \begin{displaymath}
    C(\mathbf{p})
    =
    \brace2[]{ 0.3333 - 0.5 \1 \brace1(){ 1 - p_1 \0 p_2 } }^2
    +
    \brace2[]{ 0.1667 - 0.5 \0 p_1 \0 p_2 }^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Just like with the hill climbing algorithms used in the previous post, to apply the gradient descent algorithm we need to start out with an initial estimate. Let's use the following set of parameters as the initial guess:

{% latex fig-10 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{p}
    _{\mathrlap{\sscript{start}}\phantom{\sscript{finish}}}
    =
    \begin{matrix}{l}
    0.3500
    \\[1em]
    0.2000
    \end{matrix}
    \end{displaymath}
    \end{document}
{% endlatex %}

Applying the gradient descent procedure using the above as the initial input, the output of the first iteration is the input to the second iteration. The estimate is updated repeatedly until the process terminates. Here is the final result:

{% latex fig-11 %}
    \begin{document}
    \begin{displaymath}
    \mathbf{p}
    _{\mathrlap{\sscript{finish}}\phantom{\sscript{finish}}}
    =
    \begin{matrix}{l}
    0.6140
    \\[1em]
    0.5427
    \end{matrix}
    \end{displaymath}
    \end{document}
{% endlatex %}

The algorithm terminates after 4,333 iterations. The following trace shows the path it takes from start to finish:

{% chart fig-12-trace-1-heatmap.svg %}

Using the same technique, we can run through several more examples, each with different starting points:

{% chart fig-13-trace-2-heatmap.svg %}
{% chart fig-14-trace-3-heatmap.svg %}
{% chart fig-15-trace-4-heatmap.svg %}

As you can see, the final result very much depends on the initial estimate. In each case, the path is a smooth and slightly curved line from start to finish. Notice how in each case, the trajectory is similar to that of the stochastic hill climbing algorithm used in the previous post. When using the same step size, the gradient descent approach requires around 70% to 80% of the number of iterations to complete when compared to the hill climbing alternative.

## Learning Rate

Each one of the examples illustrated above requires over a thousand iterations to complete. Some of them require over four thousand iterations. The number of iterations required depends largely on the learning rate. I think it's worth pointing out here that other learning rate schemes are possible. Some learning rates may yield a much faster convergence than the one used here. Consider this alternative:

{% latex fig-16 %}
    \begin{document}
    \begin{displaymath}
    \gamma_k = 1
    \end{displaymath}
    \end{document}
{% endlatex %}

With the cost function used in the examples above, this alternative learning rate speeds up the rate of convergence by at least two orders of magnitude. In the small toy examples illustrated here, this isn't going to make any tangible difference; it already executes fast enough. But in a larger application, an optimized learning rate can have a significant impact on the runtime performance of the gradient descent algorithm.

{% accompanying_src_link %}
