---
layout: post
title: Weighted Linear Regression
---

When doing a regression analysis, you might want to weight some data points more heavily than others. For example, when fitting a model to historic stock price data, you might want to assign more weight to recently observed price values. In this post, I demonstrate how to estimate the coefficients of a linear model using weighted least squares regression. As with the [previous post]({% post_url 2018-10-11-least-squares-and-normal-distributions %}), I also show an alternative derivation using the maximum likelihood method.

<!--excerpt-->

## Least Squares Estimation

Suppose we have a set of data points that we expect to fall on a line given by the following linear equation:

{% latex fig-01 %}
    \begin{document}
    \begin{displaymath}
    \hat{y} = a_0 + a_1 x
    \end{displaymath}
    \end{document}
{% endlatex %}

The observed data, however, contain errors for values on the vertical axis. For each data point, we define the error as the difference between the observed value and the fitted value of the linear model:

{% latex fig-02 %}
    \begin{document}
    \begin{displaymath}
    \varepsilon_i = y_i - \hat{y}_i
    \end{displaymath}
    \end{document}
{% endlatex %}

If we were performing an ordinary least squares regression, we would want to find the coefficients for the linear model that minimize the sum of the squared errors. But in this case, we want to consider the weighted sum of squares:

{% latex fig-03 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    S & = \sum_{i = 1}^{n} {w_i \varepsilon_i^2}
    \\[1em]
      & = \sum_{i = 1}^{n} {w_i (y_i - \hat{y}_i)^2}
    \\[1em]
      & = \sum_{i = 1}^{n} {w_i (y_i - a_0 - a_1 x_i)^2}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

There is a unique weight associated with the error of each observation. Some values are counted more than others, depending on the scheme used to determine the weights. Let's treat the weighted sum of squares as a function of the coefficients:

{% latex fig-04 %}
    \begin{document}
    \begin{displaymath}
    S(a_0, a_1) = {\textstyle\sum{w_i (y_i - a_0 - a_1 x_i)^2}}
    \end{displaymath}
    \end{document}
{% endlatex %}

Following the same approach we used in the [previous post]({% post_url 2018-10-11-least-squares-and-normal-distributions %}), we can estimate the coefficients of the model function by finding the values that minimize the weighted sum of squares. We take the partial derivative of the weighted sum of squares function with respect to each of the coefficients, set the derivative to zero, and then solve for the coefficient. Here are the derivatives with respect to each coefficient:

{% latex fig-05 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \frac{\partial S}{\partial a_0}
    & =
    \textstyle -2 \sum{w_i (y_i - a_0 - a_1 x_i)}
    \\[1em]
    \frac{\partial S}{\partial a_1}
    & =
    \textstyle -2 \sum{w_i (x_i y_i - a_0 x_i - a_1 x_i^2)}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Setting the derivative with respect to the first coefficient to zero, we get the following result:

{% latex fig-06 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    & \text{Let } \frac{\partial S}{\partial a_0} = 0 \text{,}
    \\[1em]
    &
    \begin{aligned}
    0 & = \textstyle \sum{w_i (y_i - a_0 - a_1 x_i)}
    \\[1em]
      & = \textstyle \sum{w_i y_i} - a_0 \sum{w_i} - a_1 \sum{w_i x_i}
    \end{aligned}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Rearranging the equation and solving for the coefficient:

{% latex fig-07 %}
    \begin{document}
    \begin{displaymath}
    a_0 = \frac{\sum{w_i y_i} - a_1 \sum{w_i x_i}}{\sum{w_i}}
    \end{displaymath}
    \end{document}
{% endlatex %}

Setting the derivative with respect to the second coefficient to zero, we get the following result:

{% latex fig-08 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    & \text{Let } \frac{\partial S}{\partial a_1} = 0 \text{,}
    \\[1em]
    &
    \begin{aligned}
    0 & = \textstyle \sum{w_i (x_i y_i - a_0 x_i - a_1 x_i^2)}
    \\[1em]
      & = \textstyle \sum{w_i x_i y_i} - a_0 \sum{w_i x_i} - a_1 \sum{w_i x_i^2}
    \end{aligned}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Rearranging the equation and solving for the coefficient:

{% latex fig-09 %}
    \begin{document}
    \begin{displaymath}
    a_1 = \frac{\sum{w_i x_i y_i} - a_0 \sum{w_i x_i}}{\sum{w_i x_i^2}}
    \end{displaymath}
    \end{document}
{% endlatex %}

If you plug in the weights and the observed values, finding the coefficients is fairly straightforward. Notice that if all the weights are equal, the result is the same as the ordinary least squares method presented in the [previous post]({% post_url 2018-10-11-least-squares-and-normal-distributions %}).

## Maximum Likelihood Estimation

Let's assume the errors are normally distributed around the model. Recall the probability density function for the normal distribution:

{% latex fig-10 %}
    \begin{document}
    \begin{displaymath}
    f(y \mid \hat{y}, \sigma)
    =
    \frac{1}{\sigma \sqrt{2 \pi}} \exp \left[- \frac{(y - \hat{y})^2}{2 \sigma^2} \right]
    \end{displaymath}
    \end{document}
{% endlatex %}

For a given set of observations, we know the likelihood of a particular mean and standard deviation value is the product of the probability density of each observation given that particular mean and standard deviation. But how do we weight one observation differently than another? For each observation, we can raise the probability density to the power of the weight associated with that observation:

{% latex fig-11 %}
    \begin{document}
    \begin{displaymath}
    L(\hat{y}, \sigma \mid y_1, y_2, \dots, y_n)
    =
    \prod_{i = 1}^{n}{f(y_i \mid \hat{y}_i, \sigma)^{w_i}}
    \end{displaymath}
    \end{document}
{% endlatex %}

If the weight of one observation is twice that of all the others, for example, then it is treated as if the measurement had appeared twice in the observed data set. The estimated mean and standard deviation values can be found by maximizing the likelihood function. To make things easier, we can work with the log-likelihood function instead:

{% latex fig-12 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \ln{L}
    & =
    \sum_{i = 1}^{n}{w_i \ln{f(y_i \mid \hat{y}_i, \sigma)}}
    \\[1em]
    & =
    \sum_{i = 1}^{n} w_i
    \left[
    - \ln{\left(\sigma \sqrt{2 \pi} \right)}
    - \frac{(y_i - \hat{y}_i)^2}{2 \sigma^2}
    \right]
    \\[1em]
    & =
    \sum_{i = 1}^{n} w_i
    \left[
    - \ln{(\sigma)}
    - \frac{1}{2} \ln{({2 \pi})}
    - \frac{(y_i - \hat{y}_i)^2}{2 \sigma^2}
    \right]
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Let's replace the mean with the body of the model function and treat the log-likelihood function as a function of the coefficients we want to solve for:

{% latex fig-13 %}
    \begin{document}
    \begin{displaymath}
    \ln{L(a_0, a_1, \sigma)}
    =
    \textstyle
    \sum w_i
    \left[
    - \ln{(\sigma)}
    - \dfrac{1}{2} \ln{(2 \pi)}
    - \dfrac{1}{2 \sigma^2} (y_i - a_0 - a_1 x_i)^2
    \right]
    \end{displaymath}
    \end{document}
{% endlatex %}

Now we can find the maximum of the log-likelihood function and solve for the coefficients using the same approach as before. Here are the partial derivatives of the log-likelihood function with respect to each of the coefficients:

{% latex fig-14 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \frac{\partial \ln{L}}{\partial a_0}
    & =
    \textstyle \dfrac{1}{\sigma^2} \sum{w_i (y_i - a_0 - a_1 x_i)}
    \\[1em]
    \frac{\partial \ln{L}}{\partial a_1}
    & =
    \textstyle \dfrac{1}{\sigma^2} \sum{w_i (x_i y_i - a_0 x_i - a_1 x_i^2)}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Setting the derivative with respect to the first coefficient to zero, we get the following result:

{% latex fig-15 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    & \text{Let } \frac{\partial \ln{L}}{\partial a_0} = 0 \text{,}
    \\[1em]
    &
    \begin{aligned}
    0 & = \textstyle \sum{w_i (y_i - a_0 - a_1 x_i)}
    \\[1em]
      & = \textstyle \sum{w_i y_i} - a_0 \sum{w_i} - a_1 \sum{w_i x_i}
    \end{aligned}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Rearranging the equation and solving for the coefficient:

{% latex fig-16 %}
    \begin{document}
    \begin{displaymath}
    a_0 = \frac{\sum{w_i y_i} - a_1 \sum{w_i x_i}}{\sum{w_i}}
    \end{displaymath}
    \end{document}
{% endlatex %}

Setting the derivative with respect to the second coefficient to zero, we get the following result:

{% latex fig-17 %}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    & \text{Let } \frac{\partial \ln{L}}{\partial a_1} = 0 \text{,}
    \\[1em]
    &
    \begin{aligned}
    0 & = \textstyle \sum{w_i (x_i y_i - a_0 x_i - a_1 x_i^2)}
    \\[1em]
      & = \textstyle \sum{w_i x_i y_i} - a_0 \sum{w_i x_i} - a_1 \sum{w_i x_i^2}
    \end{aligned}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Rearranging the equation and solving for the coefficient:

{% latex fig-18 %}
    \begin{document}
    \begin{displaymath}
    a_1 = \frac{\sum{w_i x_i y_i} - a_0 \sum{w_i x_i}}{\sum{w_i x_i^2}}
    \end{displaymath}
    \end{document}
{% endlatex %}

As expected, the weighted maximum likelihood estimation gives the same result as the weighted least squares estimation when we assume the errors are normally distributed. While I could go a step further and solve for standard deviation, I'm going to stop here. I'd like to do a more in-depth study of variances at another time.
