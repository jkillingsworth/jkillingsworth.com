---
layout: post
title: Separating Signal from Noise
---

I want to experiment with modeling price changes over time as the combination of a smooth trend component overlaid with a random noise component. My goal is to examine the statistical properties of each constituent component and compare the results to the statistical properties of the undecomposed market price.

<!--excerpt-->

In this post, I use a [least squares moving average]({% post_url 2018-09-21-least-squares-moving-averages %}) to determine the smooth component of a price series. This is our signal, so to speak. The difference between the market price and the smooth component is the noise component, which I have dubbed the "dither". All calculations are based on the logarithmic price values.

## S&P 500 ETF (Daily)

The first set of data I want to examine is a series of daily closing prices of an S&P 500 index tracking fund covering a period of approximately 21 years. The first chart below shows the market price and the smooth component. The second one shows the noise component. Here are the charts:

{% chart fig-01-stocks-daily-SPY-price.svg %}
{% chart fig-02-stocks-daily-SPY-noise.svg %}

The dither component of the daily price fluctuation is separated from the smooth component that represents the general trend, creating two distinct data sets that can be analyzed individually. In my post titled [*The Distribution of Price Fluctuations*]({% post_url 2019-01-26-the-distribution-of-price-fluctuations %}), I plotted the daily returns of the market price in a histogram, along with a fitted normal and Laplace density function. We can perform the same analysis not only on the market price data, but also on the separated smooth and dither components. See charts below:

{% chart fig-03-stocks-daily-SPY-probs-market.svg %}
{% chart fig-04-stocks-daily-SPY-probs-smooth.svg %}
{% chart fig-05-stocks-daily-SPY-probs-dither.svg %}

As might be expected, based on a [previous study]({% post_url 2019-01-26-the-distribution-of-price-fluctuations %}), the histogram for the market price data has the shape of a Laplace distribution. For the smooth data set, the shape of the histogram also looks roughly like that of a Laplace distribution, although it's a bit distorted. Notice, however, the standard deviation of the smooth data is about an order of magnitude smaller than that of the market price data. Also notice how the peak in the smooth price histogram is shifted noticeably to the right, indicating a general uptrend in the data. The rightward shift is present in the market price data as well, but it's not as noticeable because of the larger dispersion of daily price moves in the market price data.

Looking at the dither component, the shape of histogram resembles that of a Laplace distribution about as neatly as the shape of the histogram for the market price does. The standard deviation is about the same as that of the market price data as well. To gain more insights, let's look at some concrete numbers concerning the analysis of these three data sets:

{% latex fig-06 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \begin{array}{@{\rule{0em}{1.25em}}|wl{9em}|wl{7em}|wl{7em}|}
    \hline
    \text{Location Parameter} & \text{Normal ($\mu$)}    & \text{Laplace ($\mu$)}
    \\[0.25em]\hline
    \text{Market}             & +2.754 \times 10^{-4}    & +6.493 \times 10^{-4}
    \\[0.25em]\hline
    \text{Smooth}             & +2.488 \times 10^{-4}    & +4.072 \times 10^{-4}
    \\[0.25em]\hline
    \text{Dither}             & +1.921 \times 10^{-5}    & +7.902 \times 10^{-5}
    \\[0.25em]\hline
    \end{array}
    \\[1em]
    \begin{array}{@{\rule{0em}{1.25em}}|wl{9em}|wl{7em}|wl{7em}|}
    \hline
    \text{Scale Parameter}    & \text{Normal ($\sigma$)} & \text{Laplace ($b$)}
    \\[0.25em]\hline
    \text{Market}             & +1.213 \times 10^{-2}    & +8.131 \times 10^{-3}
    \\[0.25em]\hline
    \text{Smooth}             & +1.411 \times 10^{-3}    & +9.596 \times 10^{-4}
    \\[0.25em]\hline
    \text{Dither}             & +1.190 \times 10^{-2}    & +7.987 \times 10^{-3}
    \\[0.25em]\hline
    \end{array}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

These values are the maximum likelihood estimates for each data set. The tables above show the estimated location and scale parameters for both the normal density function and the Laplace density function. My post titled [*Normal and Laplace Distributions*]({% post_url 2018-11-15-normal-and-laplace-distributions %}) provides the details on how these values are calculated.

The location parameter for the normal density function is roughly the same for both the market price and the smooth component, while the estimated parameter value for the dither component is about an order of magnitude smaller. This suggests that the smooth component embodies the general direction of the price trend, while the dither component is relatively neutral. We can see a similar pattern in the location parameter values for the Laplace density function.

The scale parameter values for both density functions are roughly the same for both the market price and the dither component, while the values for the smooth component are an order of magnitude smaller. This seems to imply that the dither component embodies most of the noise that obscures the otherwise smooth trend in the original market price.

## S&P 500 ETF (Intraday)

The next set of data I want to look at is a series of intraday prices of the same S&P 500 index tracking fund evaluated previously. This data set contains one minute intraday data covering a single trading day. The charts below show the market price along with the smooth trend component and the dither noise component:

{% chart fig-07-stocks-intraday-SPY-price.svg %}
{% chart fig-08-stocks-intraday-SPY-noise.svg %}

The intraday price series contains a couple of sudden price moves that are not tracked very well by the least squares moving average. This results in large spikes on the noise chart. Let's take a look at the histogram for the market price data series and compare it to that of the separate smooth and dither components:

{% chart fig-09-stocks-intraday-SPY-probs-market.svg %}
{% chart fig-10-stocks-intraday-SPY-probs-smooth.svg %}
{% chart fig-11-stocks-intraday-SPY-probs-dither.svg %}

This histogram for market price data looks like it might approximate the shape of the Laplace density function, but it has a set of shoulders not present in the model function. The histogram for the smooth component has a shape that is even less well defined. But look at the shape of the histogram for the dither component---it looks like an almost ideal approximation of the Laplace density function. Let's take a look at the numbers:

{% latex fig-12 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \begin{array}{@{\rule{0em}{1.25em}}|wl{9em}|wl{7em}|wl{7em}|}
    \hline
    \text{Location Parameter} & \text{Normal ($\mu$)}    & \text{Laplace ($\mu$)}
    \\[0.25em]\hline
    \text{Market}             & +3.889 \times 10^{-6}    & +0.000 \times 10^{+0}
    \\[0.25em]\hline
    \text{Smooth}             & +3.579 \times 10^{-6}    & +2.765 \times 10^{-6}
    \\[0.25em]\hline
    \text{Dither}             & +7.600 \times 10^{-7}    & +1.407 \times 10^{-6}
    \\[0.25em]\hline
    \end{array}
    \\[1em]
    \begin{array}{@{\rule{0em}{1.25em}}|wl{9em}|wl{7em}|wl{7em}|}
    \hline
    \text{Scale Parameter}    & \text{Normal ($\sigma$)} & \text{Laplace ($b$)}
    \\[0.25em]\hline
    \text{Market}             & +2.499 \times 10^{-4}    & +1.374 \times 10^{-4}
    \\[0.25em]\hline
    \text{Smooth}             & +3.418 \times 10^{-5}    & +2.333 \times 10^{-5}
    \\[0.25em]\hline
    \text{Dither}             & +2.535 \times 10^{-4}    & +1.372 \times 10^{-4}
    \\[0.25em]\hline
    \end{array}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The location and scale parameters fitted to the normal density function follow the same pattern we saw in the previous data set. The smooth component represents the trend and the dither component represents the noise. For the Laplace density function, this pattern also holds for the scale parameter but not for the location parameter. Interestingly, the location parameter fitted to the Laplace density function implies a sideways trend in the market price, but indicates and upward bias in both the smooth component and the dither component.

## Japanese Yen (Daily)

Now let's take a look at the daily exchange rate between the US dollar and the Japanese yen. This data set covers a range of about 18 years. Here are the charts:

{% chart fig-13-forex-daily-USDJPY-price.svg %}
{% chart fig-14-forex-daily-USDJPY-noise.svg %}

The smooth component seems to track the market price fairly well most of the time, but there does appear to be some noticeable lag following reversals. The dither component seems to oscillate up and down in cycles. Here are the histograms:

{% chart fig-15-forex-daily-USDJPY-probs-market.svg %}
{% chart fig-16-forex-daily-USDJPY-probs-smooth.svg %}
{% chart fig-17-forex-daily-USDJPY-probs-dither.svg %}

The shape of the histogram for both the market price data and the dither component closely resemble the shape of the Laplace density function. For the smooth component, the histogram has a general bell shape, but it looks like it might be a bit too sloppy and asymmetrical to properly characterize it as having the shape of a normal or a Laplace density function. Here are the numbers:

{% latex fig-18 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \begin{array}{@{\rule{0em}{1.25em}}|wl{9em}|wl{7em}|wl{7em}|}
    \hline
    \text{Location Parameter} & \text{Normal ($\mu$)}    & \text{Laplace ($\mu$)}
    \\[0.25em]\hline
    \text{Market}             & -1.153 \times 10^{-5}    & +0.000 \times 10^{+0}
    \\[0.25em]\hline
    \text{Smooth}             & -2.138 \times 10^{-5}    & -2.425 \times 10^{-5}
    \\[0.25em]\hline
    \text{Dither}             & +5.110 \times 10^{-6}    & +4.360 \times 10^{-5}
    \\[0.25em]\hline
    \end{array}
    \\[1em]
    \begin{array}{@{\rule{0em}{1.25em}}|wl{9em}|wl{7em}|wl{7em}|}
    \hline
    \text{Scale Parameter}    & \text{Normal ($\sigma$)} & \text{Laplace ($b$)}
    \\[0.25em]\hline
    \text{Market}             & +6.169 \times 10^{-3}    & +4.385 \times 10^{-3}
    \\[0.25em]\hline
    \text{Smooth}             & +8.839 \times 10^{-4}    & +6.716 \times 10^{-4}
    \\[0.25em]\hline
    \text{Dither}             & +6.100 \times 10^{-3}    & +4.343 \times 10^{-3}
    \\[0.25em]\hline
    \end{array}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

The pattern here is very similar to that of the previous data set. These results suggest that the smooth component captures the trend and the dither component captures the noise. As with the estimates computed for the previous section, the location parameters estimated for the Laplace distribution give confusing results.

## Chinese Yuan (Intraday)

The next data set is a series of intraday exchange rates between the Chinese yuan and the US dollar covering a period of approximately 24 hours. Each data point is one minute apart. Here are the charts showing the intraday market prices along with the separated smooth and dither components:

{% chart fig-19-forex-intraday-USDCNH-price.svg %}
{% chart fig-20-forex-intraday-USDCNH-noise.svg %}

In my post titled [*The Very Strange Chinese Yuan*]({% post_url 2019-02-10-the-very-strange-chinese-yuan %}), I examined a different set of intraday exchange rates between the Chinese yuan and the US dollar. In that post, I demonstrated a peculiar triple peak pattern in the distribution of price movements. We can observe the same phenomenon in this data set as well:

{% chart fig-21-forex-intraday-USDCNH-probs-market.svg %}
{% chart fig-22-forex-intraday-USDCNH-probs-smooth.svg %}
{% chart fig-23-forex-intraday-USDCNH-probs-dither.svg %}

The shape of the histogram for the market price data shows the triple peak pattern that is characteristic of intraday exchange rates between the yuan and dollar. The histogram for the smooth component exhibits a roughly bell shaped distribution with no indication of the triple peak pattern at all. The fitted density functions for the smooth component are both shifted to the left, which can be attributed to the downward trend visible in the price chart. The histogram for the dither component, on the other hand, clearly shows the triple peak pattern, indicating that this distinctive noise pattern is almost entirely removed from the price trend. Here are the parameter estimates for the density functions:

{% latex fig-24 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \begin{array}{@{\rule{0em}{1.25em}}|wl{9em}|wl{7em}|wl{7em}|}
    \hline
    \text{Location Parameter} & \text{Normal ($\mu$)}    & \text{Laplace ($\mu$)}
    \\[0.25em]\hline
    \text{Market}             & -2.133 \times 10^{-6}    & +0.000 \times 10^{+0}
    \\[0.25em]\hline
    \text{Smooth}             & -2.374 \times 10^{-6}    & -1.598 \times 10^{-6}
    \\[0.25em]\hline
    \text{Dither}             & -3.132 \times 10^{-7}    & -1.345 \times 10^{-6}
    \\[0.25em]\hline
    \end{array}
    \\[1em]
    \begin{array}{@{\rule{0em}{1.25em}}|wl{9em}|wl{7em}|wl{7em}|}
    \hline
    \text{Scale Parameter}    & \text{Normal ($\sigma$)} & \text{Laplace ($b$)}
    \\[0.25em]\hline
    \text{Market}             & +1.894 \times 10^{-4}    & +1.216 \times 10^{-4}
    \\[0.25em]\hline
    \text{Smooth}             & +6.481 \times 10^{-6}    & +5.006 \times 10^{-6}
    \\[0.25em]\hline
    \text{Dither}             & +1.899 \times 10^{-4}    & +1.223 \times 10^{-4}
    \\[0.25em]\hline
    \end{array}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Again, we see a pattern here similar to that of the previous data sets. For the parameters fitted to the normal density function, the magnitude of the values give evidence that the smooth component represents the trend and the dither component represents the noise. We can see this mirrored in the scale parameter values fitted to the Laplace density function, but the fitted location parameter values are not as intuitive. For the Laplace density function, the location parameter fitted to the market price data is zero even though there is an obvious downward trend in the price chart.

## Bitcoin (Daily)

The final set of data I want to examine are the daily Bitcoin prices covering a period of about five years. Here are the charts:

{% chart fig-25-crypto-daily-BTC-price.svg %}
{% chart fig-26-crypto-daily-BTC-noise.svg %}

The price chart shows a fairly consistent multi-year trend followed by a distinct reversal. The noise chart exhibits what appears to be a cyclical pattern, although the periods don't seem to be evenly spaced. Here are the histograms:

{% chart fig-27-crypto-daily-BTC-probs-market.svg %}
{% chart fig-28-crypto-daily-BTC-probs-smooth.svg %}
{% chart fig-29-crypto-daily-BTC-probs-dither.svg %}

The histograms for both the market price and the dither component have a shape that resembles the Laplace density function. The histogram for the smooth component has a sloppy and irregular shape. Here are the numbers:

{% latex fig-30 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{aligned}
    \begin{array}{@{\rule{0em}{1.25em}}|wl{9em}|wl{7em}|wl{7em}|}
    \hline
    \text{Location Parameter} & \text{Normal ($\mu$)}    & \text{Laplace ($\mu$)}
    \\[0.25em]\hline
    \text{Market}             & +1.285 \times 10^{-3}    & +1.550 \times 10^{-3}
    \\[0.25em]\hline
    \text{Smooth}             & +1.214 \times 10^{-3}    & +6.953 \times 10^{-4}
    \\[0.25em]\hline
    \text{Dither}             & +3.659 \times 10^{-4}    & +1.532 \times 10^{-3}
    \\[0.25em]\hline
    \end{array}
    \\[1em]
    \begin{array}{@{\rule{0em}{1.25em}}|wl{9em}|wl{7em}|wl{7em}|}
    \hline
    \text{Scale Parameter}    & \text{Normal ($\sigma$)} & \text{Laplace ($b$)}
    \\[0.25em]\hline
    \text{Market}             & +3.855 \times 10^{-2}    & +2.476 \times 10^{-2}
    \\[0.25em]\hline
    \text{Smooth}             & +5.870 \times 10^{-3}    & +4.548 \times 10^{-3}
    \\[0.25em]\hline
    \text{Dither}             & +3.832 \times 10^{-2}    & +2.477 \times 10^{-2}
    \\[0.25em]\hline
    \end{array}
    \end{aligned}
    \end{displaymath}
    \end{document}
{% endlatex %}

Not surprisingly, these results mirror what we've seen with the other data sets. The parameters fitted to the normal density function and the scale parameters fitted to the Laplace density function indicate that the smooth and dither components represent the trend and noise respectively. The location parameters estimated for the Laplace density function remain a bit more mysterious.

## Final Thoughts

I think the most obvious conclusion to draw from this experiment is that a smooth price signal can be separated from the unrelated noise in price fluctuations. More specifically, the particular shape of the distribution of price movements---a shape that typically resembles that of a Laplace distribution---can be detached from the smooth price trend. The characteristics of the distribution of market price fluctuations are largely a consequence of the noise component independent of the trend component. Even in the case of the intraday Chinese yuan prices, the idiosyncratic triple peak distribution can be isolated to the noise component only.

Another interesting observation in the data sets examined here is that the noise is not entirely random. There is a structure to it. There are undeniable up and down cycles. My initial thought is to apply Fourier analysis to extract a cyclical component from the residual noise. I am curious what the distribution characteristics of the residual noise would look like if it could be isolated from the cyclical component as well as the trend component. The up and down cycles are somewhat irregular, however, which might make a Fourier analysis difficult. I think this is something worth further investigation.

The techniques used in this article rely on a least squares moving average to determine the smooth trend component of a price series. While the least squares moving average is great for tracking sustained price trends, the disadvantage is that it reacts slowly to sharp reversals in the trend. The slow reaction to trend reversals can produce artificially large spikes in the noise component. There might be better smoothing algorithms worth exploring.

{% accompanying_src_link %}
