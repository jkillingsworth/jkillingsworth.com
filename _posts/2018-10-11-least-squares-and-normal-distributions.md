---
layout: post
title: Least Squares and Normal Distributions
---

The method of least squares estimates the coefficients of a model function by minimizing the sum of the squared errors between the model and the observed values. In this post, I show the derivation of the parameter estimates for a linear model. In addition, I show that the maximum likelihood estimation is the same as the least squares estimation when we assume the errors are normally distributed.

<!--excerpt-->

## Least Squares Estimation

Suppose we have a set of two-dimensional data points that we observed by measuring some kind of phenomenon:

{% latex fig-01 %}
    \begin{document}
    \begin{displaymath}
    \{\, (x_1, y_1), (x_2, y_2), \dots, (x_n, y_n) \,\}
    \end{displaymath}
    \end{document}
{% endlatex %}

Also, suppose that the measuring device is inaccurate. The data we observed contain errors for values on the vertical axis. Despite the errors, we know the correct readings fall somewhere on a line given by the following linear equation:

{% latex fig-02 %}
    \begin{document}
    \begin{displaymath}
    \hat{y} = a_0 + a_1 x
    \end{displaymath}
    \end{document}
{% endlatex %}

For each data point, we can compute the error as the difference between the observed value and the correct value according to the model function:

{% latex fig-03 %}
    \begin{document}
    \begin{displaymath}
    \varepsilon_i = y_i - \hat{y}_i
    \end{displaymath}
    \end{document}
{% endlatex %}

The errors can be positive or negative. Taking the square of each error always yields a positive number. We can define the sum of the squared errors like this:

{% latex fig-04 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    S & = \sum_{i = 1}^{n} \varepsilon_i^2
    \\[1em]
      & = \sum_{i = 1}^{n} \2 (y_i - \hat{y}_i)^2
    \\[1em]
      & = \sum_{i = 1}^{n} \2 (y_i - a_0 - a_1 x_i)^2
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Since the coefficients are unknown variables, we can treat the sum of the squared errors as a function of the coefficients:

{% latex fig-05 %}
    \begin{document}
    \begin{displaymath}
    S(a_0, a_1) = \tsum \2 (y_i - a_0 - a_1 x_i)^2
    \end{displaymath}
    \end{document}
{% endlatex %}

To estimate the coefficients of the model function using the least squares method, we need to figure out what values for the coefficients give us the smallest value for the sum of the squared errors. We can find the minimum by first taking the partial derivative of the sum of squares function with respect to each of the coefficients, setting the derivative to zero, and then solving for the coefficient. Here are the derivatives with respect to each coefficient:

{% latex fig-06 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \frac{\pderiv S}{\pderiv a_0}
    & =
    -2 \tsum \2 (y_i - a_0 - a_1 x_i)
    \\[1em]
    \frac{\pderiv S}{\pderiv a_1}
    & =
    -2 \tsum \2 (x_i y_i - a_0 x_i - a_1 x_i^2)
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Setting the derivative with respect to the first coefficient to zero, we get the following result:

{% latex fig-07 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    & \text{Let } \frac{\pderiv S}{\pderiv a_0} = 0 \text{,}
    \\[1em]
    &
    \begin{aligned}
    0 & = \tsum \2 (y_i - a_0 - a_1 x_i)
    \\[1em]
      & = \tsum y_i - n a_0 - a_1 \tsum x_i
    \end{aligned}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Rearranging the equation and solving for the coefficient:

{% latex fig-08 %}
    \begin{document}
    \begin{displaymath}
    a_0 = \frac{\tsum y_i - a_1 \tsum x_i}{n}
    \end{displaymath}
    \end{document}
{% endlatex %}

Setting the derivative with respect to the second coefficient to zero, we get the following result:

{% latex fig-09 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    & \text{Let } \frac{\pderiv S}{\pderiv a_1} = 0 \text{,}
    \\[1em]
    &
    \begin{aligned}
    0 & = \tsum \2 (x_i y_i - a_0 x_i - a_1 x_i^2)
    \\[1em]
      & = \tsum x_i y_i - a_0 \tsum x_i - a_1 \tsum x_i^2
    \end{aligned}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Rearranging the equation and solving for the coefficient:

{% latex fig-10 %}
    \begin{document}
    \begin{displaymath}
    a_1 = \frac{\tsum x_i y_i - a_0 \tsum x_i}{\tsum x_i^2}
    \end{displaymath}
    \end{document}
{% endlatex %}

Each one of the coefficients is given in terms of the other. Since there are two equations and two unknowns, you can plug one into the other to derive the final outcome. Another way to do this might be to treat the results as a system of linear equations arranged as follows:

{% latex fig-11 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    (n) \1 a_0 + \brace1(){\tsum x_i^{\phantom{1}}} \1 a_1
    & =
    \tsum y_i
    \\[1em]
    \brace1(){\tsum x_i^{\phantom{1}}} \1 a_0 + \brace1(){\tsum x_i^2} \1 a_1
    & =
    \tsum x_i y_i
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The coefficients can then be found by solving the following matrix equation:

{% latex fig-12 %}
    \begin{document}
    \begin{displaymath}
    \begin{matrix}{l}
    a_0
    \\[1em]
    a_1
    \end{matrix}
    =
    \begin{matrix}{ll}
    n             & \tsum x_i^{ }
    \\[1em]
    \tsum x_i^{ } & \tsum x_i^{2}
    \end{matrix}
    ^{-1}
    \begin{matrix}{l}
    \tsum y_i^{ }
    \\[1em]
    \tsum x_i^{\phantom{1}} y_i^{ }
    \end{matrix}
    \end{displaymath}
    \end{document}
{% endlatex %}

This method is perhaps a cleaner approach. It can also work well for model functions with many coefficients, such as higher order polynomials or multivariable functions.

## Maximum Likelihood Estimation

Now let's assume the errors are normally distributed. That is to say, the observed values are normally distributed around the model. The probability density function for the normal distribution looks like this:

{% latex fig-13 %}
    \begin{document}
    \begin{displaymath}
    f(y \mid \hat{y}, \sigma)
    =
    \frac{1}{\sigma \sqrt{2 \pi}} \exp \2 \brace4[]{- \frac{(y - \hat{y})^2}{2 \sigma^2}}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here we treat our model as the mean. We also consider the standard deviation, depicted by sigma, which measures the spread of the data around the mean. Given our observed data points, we want to figure out what the most likely values are for the mean and standard deviation. For a single data point alone, the likelihood function for a given mean and standard deviation is:

{% latex fig-14 %}
    \begin{document}
    \begin{displaymath}
    L(\hat{y}, \sigma \mid y)
    =
    f(y \mid \hat{y}, \sigma)
    \end{displaymath}
    \end{document}
{% endlatex %}

The likelihood is equal to the probability density. For all data points combined, the likelihood function for a given mean and standard deviation is equal to the product of the density at each individual data point:

{% latex fig-15 %}
    \begin{document}
    \begin{displaymath}
    L(\hat{y}, \sigma \mid y_1, y_2, \dots, y_n)
    =
    \prod_{i = 1}^{n} f(y_i \mid \hat{y}_i, \sigma)
    \end{displaymath}
    \end{document}
{% endlatex %}

At this point, we just need to find the mean and standard deviation values that maximize the likelihood function. Similar to what we did in the previous section, we can find the maximum by taking the partial derivative of the likelihood function with respect to each of the coefficients, setting the derivative to zero, and then solving for the coefficients. This might be easier to do if we first take the natural logarithm of the likelihood function:

{% latex fig-16 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \ln L
    & =
    \sum_{i = 1}^{n} \ln f(y_i \mid \hat{y}_i, \sigma)
    \\[1em]
    & =
    \sum_{i = 1}^{n}
    \2
    \brace4[]
    {
    - \ln \2 \brace1(){\sigma \sqrt{2 \pi}}
    - \frac{(y_i - \hat{y}_i)^2}{2 \sigma^2}
    }
    \\[1em]
    & =
    \sum_{i = 1}^{n}
    \2
    \brace4[]
    {
    - \ln \2 (\sigma)
    - \frac{1}{2} \ln \2 (2 \pi)
    - \frac{(y_i - \hat{y}_i)^2}{2 \sigma^2}
    }
    \\[1em]
    & =
    - n \ln \2 (\sigma)
    - \frac{n}{2} \ln \2 (2 \pi)
    - \frac{1}{2 \sigma^2} \sum_{i = 1}^{n} \2 (y_i - \hat{y}_i)^2
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Since we're interested in finding the coefficients of the model function, we can replace the mean parameter with the body of the model function and treat the likelihood function as a function of the coefficients:

{% latex fig-17 %}
    \begin{document}
    \begin{displaymath}
    \ln L(a_0, a_1, \sigma)
    =
    - n \ln \2 (\sigma)
    - \frac{n}{2} \ln \2 (2 \pi)
    - \frac{1}{2 \sigma^2} \tsum \2 (y_i - a_0 - a_1 x_i)^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Let's call this the log-likelihood function. Since the natural logarithm function is a monotonically increasing function, we can maximize the log-likelihood function and get the same result we would get if we maximized the original likelihood function. Here are the partial derivatives of the log-likelihood function with respect to each of the coefficients:

{% latex fig-18 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \frac{\pderiv \ln L}{\pderiv a_0}
    & =
    \frac{1}{\sigma^2} \tsum \2 (y_i - a_0 - a_1 x_i)
    \\[1em]
    \frac{\pderiv \ln L}{\pderiv a_1}
    & =
    \frac{1}{\sigma^2} \tsum \2 (x_i y_i - a_0 x_i - a_1 x_i^2)
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Setting the derivative with respect to the first coefficient to zero, we get the following result:

{% latex fig-19 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    & \text{Let } \frac{\pderiv \ln L}{\pderiv a_0} = 0 \text{,}
    \\[1em]
    &
    \begin{aligned}
    0 & = \tsum \2 (y_i - a_0 - a_1 x_i)
    \\[1em]
      & = \tsum y_i - n a_0 - a_1 \tsum x_i
    \end{aligned}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Rearranging the equation and solving for the coefficient:

{% latex fig-20 %}
    \begin{document}
    \begin{displaymath}
    a_0 = \frac{\tsum y_i - a_1 \tsum x_i}{n}
    \end{displaymath}
    \end{document}
{% endlatex %}

Setting the derivative with respect to the second coefficient to zero, we get the following result:

{% latex fig-21 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    & \text{Let } \frac{\pderiv \ln L}{\pderiv a_1} = 0 \text{,}
    \\[1em]
    &
    \begin{aligned}
    0 & = \tsum \2 (x_i y_i - a_0 x_i - a_1 x_i^2)
    \\[1em]
      & = \tsum x_i y_i - a_0 \tsum x_i - a_1 \tsum x_i^2
    \end{aligned}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Rearranging the equation and solving for the coefficient:

{% latex fig-22 %}
    \begin{document}
    \begin{displaymath}
    a_1 = \frac{\tsum x_i y_i - a_0 \tsum x_i}{\tsum x_i^2}
    \end{displaymath}
    \end{document}
{% endlatex %}

As you can see, we get at the same results we got from using the method of least squares to estimate the coefficients. For completeness, the same procedure can be used to find the standard deviation:

{% latex fig-23 %}
    \begin{document}
    \begin{displaymath}
    \frac{\pderiv \ln L}{\pderiv \sigma}
    =
    - \frac{n}{\sigma}
    + \frac{1}{\sigma^3} \tsum \2 (y_i - a_0 - a_1 x_i)^2
    \end{displaymath}
    \end{document}
{% endlatex %}

Setting the derivative to zero, we get the following result for the standard deviation:

{% latex fig-24 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    & \text{Let } \frac{\pderiv \ln L}{\pderiv \sigma} = 0 \text{,}
    \\[1em]
    &
    \begin{aligned}
    0 & = - \frac{n}{\sigma} + \frac{1}{\sigma^3} \tsum \2 (y_i - a_0 - a_1 x_i)^2
    \\[1em]
      & = - n \sigma^2 + \tsum \2 (y_i - a_0 - a_1 x_i)^2
    \end{aligned}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Rearranging the equation and solving for sigma:

{% latex fig-25 %}
    \begin{document}
    \begin{displaymath}
    \sigma = \sqrt{\displaystyle \frac{1}{n} \tsum \2 (y_i - a_0 - a_1 x_i)^2}
    \end{displaymath}
    \end{document}
{% endlatex %}

Note that this result may yield a biased estimate of the standard deviation when computing the value based on a limited number of samples. It might be more appropriate to use an unbiased estimator that takes the number of degrees of freedom into consideration. But that's out of scope for this post. Perhaps it's a topic I'll explore at another time.
