---
layout: post
title: Distributions on a Logarithmic Scale
---

In this post, I want to explore the logarithmic analogues of the normal and Laplace distributions. We can define a log-normal probability distribution as a distribution in which its logarithm is normally distributed. Likewise, a log-Laplace distribution is a distribution whose logarithm has a Laplace distribution. If we have a given probability density function, how can we determine its logarithmic equivalent?

<!--excerpt-->

## Determining the Logarithmic Equivalent

Suppose we have a continuous random variable. We can define the cumulative distribution function of the random variable like this:

{% latex fig-01 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    F(x) = Pr(X \leq x)
    \end{displaymath}
    \end{document}
{% endlatex %}

Let's also assume we know the probability density function of the random variable. The density function is the derivative of the cumulative distribution function. We can define the cumulative distribution function based on the probability density function like this:

{% latex fig-02 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    F(x) = \lim_{a \to -\infty} \int_{a}^{x}{f(t)\,dt}
    \end{displaymath}
    \end{document}
{% endlatex %}

The probability of observing a realization of the random variable in a range between two points can be expressed like this:

{% latex fig-03 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    F(x) \Big|_{a}^{b} = F(b) - F(a) = \int_{a}^{b}{f(x)\,dx}
    \end{displaymath}
    \end{document}
{% endlatex %}

Now suppose we have two continuous random variables. The probability distribution of one is the logarithm of the other:

{% latex fig-04 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    U = \ln{X}
    \end{displaymath}
    \end{document}
{% endlatex %}

Our goal is to derive the density function of one based on the density function of the other. Let's use the following notation:

{% latex fig-05 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    f(u) & =
    \frac{d}{du} F(u) & \text{is the probability density of $U$}
    \\[1em]
    g(x) & =
    \frac{d}{dx} G(x) & \text{is the probability density of $X$}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

With this notation, we can express the relationship between these two distributions using the following equation:

{% latex fig-06 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    G(x) \Big|_{a}^{b} = F(u) \Big\rvert_{\ln{a}}^{\ln{b}}
    \end{displaymath}
    \end{document}
{% endlatex %}

The substitution rule for integration can be used to evaluate this further. Let's consider the following substitution:

{% latex fig-07 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    u = \ln{x}
    \end{displaymath}
    \end{document}
{% endlatex %}

Let's also consider its derivative:

{% latex fig-08 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    \frac{du}{dx} = \frac{1}{x}
    \end{displaymath}
    \end{document}
{% endlatex %}

Plugging in the substitution, we can compute the probability of observing the random variable between two points on a logarithmic scale like this:

{% latex fig-09 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    F(u) \Big|_{\ln{a}}^{\ln{b}} = F(\ln{b}) - F(\ln{a}) = \int_{\ln{a}}^{\ln{b}}{f(u)\,du}
    \end{displaymath}
    \end{document}
{% endlatex %}

The substitution rule for definite integrals gives us the following identity:

{% latex fig-10 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    \int_{\ln{a}}^{\ln{b}}{f(u)\,du} = \int_{a}^{b}{\frac{1}{x}f(\ln{x})\,dx}
    \end{displaymath}
    \end{document}
{% endlatex %}

With this, we can compute the same probability of observing the random variable between two points, but this time on a linear scale:

{% latex fig-11 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    G(x) \Big|_{a}^{b} = G(b) - G(a) = \int_{a}^{b}{\frac{1}{x}f(\ln{x})\,dx}
    \end{displaymath}
    \end{document}
{% endlatex %}

We can now state the following solution:

{% latex fig-12 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    g(x) = \frac{1}{x}f(\ln{x})
    \end{displaymath}
    \end{document}
{% endlatex %}

Using these steps, we can determine the logarithmic equivalent of any continuous distribution for which we know the formula for the probability density function.

## The Log-Normal Distribution

To give an example, we can use the probability density function for the normal distribution to determine the probability density for the log-normal distribution. Recall the density function for the normal distribution:

{% latex fig-13 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    f(x \mid \mu, \sigma)
    =
    \frac{1}{\sigma \sqrt{2 \pi}} \exp \left[- \frac{(x - \mu)^2}{2 \sigma^2} \right]
    \end{displaymath}
    \end{document}
{% endlatex %}

The logarithmic equivalent is:

{% latex fig-14 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    g(x \mid \mu, \sigma)
    =
    \frac{1}{x \sigma \sqrt{2 \pi}} \exp \left[- \frac{(\ln{x} - \mu)^2}{2 \sigma^2} \right]
    \end{displaymath}
    \end{document}
{% endlatex %}

If we have a set of samples of a random variable that we know the have a log-normal distribution, the parameters of the distribution can be estimated using the maximum likelihood method outlined in my previous post. I'll skip the intermediate steps and jump straight to the results.

Here is the estimate for the mean:

{% latex fig-15 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    \hat{\mu} = \frac{1}{n} \sum_{i = 1}^{n}{\ln{x_i}}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is the estimate for the standard deviation:

{% latex fig-16 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    \hat{\sigma} = \sqrt{\frac{1}{n} \sum_{i = 1}^{n}{(\ln{x_i} - \mu)^2}}
    \end{displaymath}
    \end{document}
{% endlatex %}

Not surprisingly, the formulas to compute the parameter estimates for the log-normal distribution are nearly the same as those of the normal distribution. The only difference is that we take the logarithm of the observed data points.

## The Log-Laplace Distribution

The logarithmic equivalent of the Laplace distribution can be found in the same way as the logarithmic equivalent of the normal distribution. Consider the probability density function for the Laplace distribution:

{% latex fig-17 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    f(x \mid \mu, b)
    =
    \frac{1}{2b} \exp \left(- \frac{|x - \mu|}{b} \right)
    \end{displaymath}
    \end{document}
{% endlatex %}

The logarithmic equivalent is:

{% latex fig-18 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    g(x \mid \mu, b)
    =
    \frac{1}{2bx} \exp \left(- \frac{|\ln{x} - \mu|}{b} \right)
    \end{displaymath}
    \end{document}
{% endlatex %}

If we have a set of samples of a random variable that we know to have a log-Laplace distribution, the parameters can be estimated as before using the maximum likelihood method. You can see my previous post for full details. We first need to rank the samples in ascending order:

{% latex fig-19 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    \{\ x_1, x_2, \dots, x_{m}, \dots, x_{n-1}, x_n \mid x_{i} \leq x_{i+1}\ \}
    \end{displaymath}
    \end{document}
{% endlatex %}

We also need to determine the middle value:

{% latex fig-20 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    m
    =
    \begin{dcases}
    \frac{n}{2}     & \quad \text{if $n$ is even}
    \\[0.5em]
    \frac{n + 1}{2} & \quad  \text{if $n$ is odd}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is the estimate for the location parameter:

{% latex fig-21 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    \hat{\mu}
    =
    \begin{dcases}
    \frac{1}{2} (\ln{x_{m}} + \ln{x_{m+1}}) & \quad \text{if $n$ is even}
    \\[0.5em]
    \ln{x_{m}}                              & \quad \text{if $n$ is odd}
    \end{dcases}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here is the estimate for the scale parameter:

{% latex fig-22 %}
    \documentclass[varwidth]{standalone}
    \usepackage{mathtools}
    \begin{document}
    \begin{displaymath}
    \hat{b} = \frac{1}{n} \sum_{i = 1}^{n}{|\ln{x_i} - \mu|}
    \end{displaymath}
    \end{document}
{% endlatex %}

Once again, the formulas to compute the parameter estimates for the log-Laplace distribution are nearly the same as those of the regular Laplace distribution. For the logarithmic equivalent, we simply take the logarithm of the observed data points.

## Comparison

When plotted on a graph, the probability density function for the log-normal distribution looks like a distorted version of the normal distribution's density function. This isn't too surprising. What's interesting to me, however, is the shape of the log-Laplace density function. It looks like a skateboard ramp:

{% chart fig-23-distributions-lin.svg %}

The flat top of the log-Laplace density function looks peculiar. I wasn't expecting it, and I initially thought I had made a mistake when generating the chart. However, the flat part only exists when the scale parameter is set to the standard value of one. The shape of the graph changes as the scale parameter is adjusted up or down. Take a look at the same chart when the horizontal axis has a logarithmic scale:

{% chart fig-24-distributions-log.svg %}

Notice how the shape of the log-normal density function looks very much like the symmetrical shape of the regular normal distribution. This trait does not exist for the log-Laplace distribution, however. When plugging in smaller values for the scale parameter, the shape of the log-Laplace density function tends to have a closer resemblance to that of the regular Laplace distribution, but it doesn't exhibit the same symmetry.
