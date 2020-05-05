---
layout: post
title: The Distribution of Price Fluctuations
---

Are price fluctuations in the financial markets normally distributed? If I understand history correctly, it was French mathematician Louis Bachelier who was the first to explore this topic over 100 years ago. While Bachelier's work assumed that price movements were normally distributed, a mathematician named Benoit Mandelbrot made some interesting observations that suggest otherwise.

<!--excerpt-->

In the 1960s, Mandelbrot studied historical cotton prices and noticed the distribution of price changes did not exhibit properties characteristic of a normal distribution. Thankfully, computer technology and the availability of historical price data have improved drastically since the 1960s. Doing this sort of analysis is much easier today than it was in the past. In this post, I set out to perform my own analysis of the distribution of price fluctuations across a variety of different markets.

## Analyzing the Data

The first set of data I want to look at are the daily closing prices of an S&P 500 index tracking fund. Specifically, I'm using the SPDR S&P 500 ETF. The ticker symbol is SPY. This is currently one of the most heavily traded instruments on the New York Stock Exchange. Below is a chart of the daily closing prices over the past 21 years:

{% chart fig-01-stocks-daily-SPY-price.svg %}

Notice in the chart above that the vertical axis represents the natural logarithm of the closing price, not the actual closing price itself. We are concerned with the variance of price change percentages here and not the change in real price, so we're going to analyze the logarithmic values in this study instead of the actual price values. The chart below shows the differences in the logarithmic price values from one day to the next:

{% chart fig-02-stocks-daily-SPY-diffs.svg %}

The price moves up and down seemingly at random. The magnitude of each price move varies from day to day. Are these variations in price movement normally distributed? It's hard to tell just by looking at the chart. What stands out, however, is that the variance seems to be heteroskedastic; there are periods of relative calm followed by clusters of large moves up and down. While a study of heteroskedasticity is beyond the scope of this post, it is definitely a peculiarity worth pointing out.

Ignoring these pockets of high and low variability, the question remains: is the overall variation in price movement normally distributed? Would the data better conform to a different probability distribution instead? Let's put the data in a histogram and see what it looks like:

{% chart fig-03-stocks-daily-SPY-probs.svg %}

The histogram is overlaid with two probability density functions, one representing a normal distribution and the other representing a Laplace distribution. The parameters for each distribution were estimated using the maximum likelihood method. You can see my post titled [*Normal and Laplace Distributions*]({% post_url 2018-11-15-normal-and-laplace-distributions %}) for details on how to compute the maximum likelihood estimates.

So are the price fluctuations normally distributed? Eyeballing the chart above, it appears not. At least for this data set. Without measuring it objectively, it certainly looks like the Laplace distribution is a better fit. But what happens if we perform this experiment on different data sets? Will we get the same results for different asset types? What if we study intraday data instead of daily closing prices? The following sections present the result of this analysis across a mix of different markets and time frames.

## Exchange Traded Funds

To get some broader insights regarding the behavior of price movements, I want to take a look at some more exchange traded funds with a few different underlying asset types. Let's examine the daily closing prices for the following ETFs:

{% latex fig-04 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|>{$}wl{4em}<{$}|>{$}wl{15em}<{$}|}
    \hline
    \text{Symbol}    & \text{Underlying Asset}
    \\[0.25em]\hline
    \texttt{DIA}     & \text{Large cap domestic stocks}
    \\[0.25em]\hline
    \texttt{EEM}     & \text{Emerging markets}
    \\[0.25em]\hline
    \texttt{GLD}     & \text{Gold}
    \\[0.25em]\hline
    \texttt{HYG}     & \text{High yield corporate bonds}
    \\[0.25em]\hline
    \texttt{LQD}     & \text{Investment grade corporate bonds}
    \\[0.25em]\hline
    \texttt{TLT}     & \text{Long term US Treasury bonds}
    \\[0.25em]\hline
    \texttt{UNG}     & \text{Natural gas}
    \\[0.25em]\hline
    \texttt{USO}     & \text{Crude oil}
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

Each data set contains at least 10 years worth of data. Using the same technique as before, we can plot the histogram of daily price fluctuations and overlay the fitted normal and Laplace density functions. Here are the charts:

{% chart fig-05-stocks-daily-DIA-probs.svg %}
{% chart fig-06-stocks-daily-EEM-probs.svg %}
{% chart fig-07-stocks-daily-GLD-probs.svg %}
{% chart fig-08-stocks-daily-HYG-probs.svg %}
{% chart fig-09-stocks-daily-LQD-probs.svg %}
{% chart fig-10-stocks-daily-TLT-probs.svg %}
{% chart fig-11-stocks-daily-UNG-probs.svg %}
{% chart fig-12-stocks-daily-USO-probs.svg %}

As you can see, the histogram seems to fit the Laplace distribution better than the normal distribution most of the time. But not always. There are some cases that appear to fit somewhere in between the normal distribution and the Laplace distribution. A cursory look at UNG, for example, might suggest the variation is normally distributed.

## Individual Stocks (Daily)

Instead of looking at broad stock market indexes, let's see what happens if we examine individual stocks. Let's consider the daily closing prices for the following stocks:

{% latex fig-13%}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|>{$}wl{4em}<{$}|>{$}wl{15em}<{$}|}
    \hline
    \text{Symbol}    & \text{Underlying Asset}
    \\[0.25em]\hline
    \texttt{AMZN}    & \text{Amazon.com, Inc.}
    \\[0.25em]\hline
    \texttt{AZO}     & \text{AutoZone, Inc.}
    \\[0.25em]\hline
    \texttt{BLK}     & \text{BlackRock, Inc.}
    \\[0.25em]\hline
    \texttt{CAT}     & \text{Caterpillar Inc.}
    \\[0.25em]\hline
    \texttt{CMG}     & \text{Chipotle Mexican Grill, Inc.}
    \\[0.25em]\hline
    \texttt{FDX}     & \text{FedEx Corporation}
    \\[0.25em]\hline
    \texttt{GM}      & \text{General Motors Company}
    \\[0.25em]\hline
    \texttt{GOOG}    & \text{Alphabet Inc.}
    \\[0.25em]\hline
    \texttt{GWW}     & \text{W.W. Grainger, Inc.}
    \\[0.25em]\hline
    \texttt{HUM}     & \text{Humana Inc.}
    \\[0.25em]\hline
    \texttt{NFLX}    & \text{Netflix, Inc.}
    \\[0.25em]\hline
    \texttt{TSLA}    & \text{Tesla, Inc.}
    \\[0.25em]\hline
    \texttt{TWLO}    & \text{Twilio Inc.}
    \\[0.25em]\hline
    \texttt{ULTA}    & \text{Ulta Beauty, Inc.}
    \\[0.25em]\hline
    \texttt{UNH}     & \text{UnitedHealth Group Incorporated}
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here are the charts:

{% chart fig-14-stocks-daily-AMZN-probs.svg %}
{% chart fig-15-stocks-daily-AZO-probs.svg %}
{% chart fig-16-stocks-daily-BLK-probs.svg %}
{% chart fig-17-stocks-daily-CAT-probs.svg %}
{% chart fig-18-stocks-daily-CMG-probs.svg %}
{% chart fig-19-stocks-daily-FDX-probs.svg %}
{% chart fig-20-stocks-daily-GM-probs.svg %}
{% chart fig-21-stocks-daily-GOOG-probs.svg %}
{% chart fig-22-stocks-daily-GWW-probs.svg %}
{% chart fig-23-stocks-daily-HUM-probs.svg %}
{% chart fig-24-stocks-daily-NFLX-probs.svg %}
{% chart fig-25-stocks-daily-TSLA-probs.svg %}
{% chart fig-26-stocks-daily-TWLO-probs.svg %}
{% chart fig-27-stocks-daily-ULTA-probs.svg %}
{% chart fig-28-stocks-daily-UNH-probs.svg %}

Subjectively, it seems like the Laplace distribution is a better overall fit.

## Individual Stocks (Intraday)

What happens if we study intraday stock prices instead of daily closing prices? Let's perform the analysis on the same set of stocks using one minute price data instead of end of day prices. Here are the charts:

{% chart fig-29-stocks-intraday-AMZN-probs.svg %}
{% chart fig-30-stocks-intraday-AZO-probs.svg %}
{% chart fig-31-stocks-intraday-BLK-probs.svg %}
{% chart fig-32-stocks-intraday-CAT-probs.svg %}
{% chart fig-33-stocks-intraday-CMG-probs.svg %}
{% chart fig-34-stocks-intraday-FDX-probs.svg %}
{% chart fig-35-stocks-intraday-GM-probs.svg %}
{% chart fig-36-stocks-intraday-GOOG-probs.svg %}
{% chart fig-37-stocks-intraday-GWW-probs.svg %}
{% chart fig-38-stocks-intraday-HUM-probs.svg %}
{% chart fig-39-stocks-intraday-NFLX-probs.svg %}
{% chart fig-40-stocks-intraday-TSLA-probs.svg %}
{% chart fig-41-stocks-intraday-TWLO-probs.svg %}
{% chart fig-42-stocks-intraday-ULTA-probs.svg %}
{% chart fig-43-stocks-intraday-UNH-probs.svg %}

Each data set contains five days worth of data. Using intraday data instead of daily data doesn't seem to change the outcome with respect to the general shape of the distribution.

## Volatility Index

I'm curious what happens if we apply this analysis to the values of the CBOE Volatility Index. This index is a measure of implied volatility based on the market price of S&P 500 options. While not a tradable instrument in its own right, there are tradable derivatives based on this index. Here is a chart of daily closing values covering a span of about 19 years:

{% chart fig-44-stocks-daily-VIX-price.svg %}

Consistent with the other data sets used in this post, the "price" values on this chart are the logarithmic values of the volatility index, not the actual index values. The VIX is a quoted in percentage points, so the use of logarithmic values may or may not be the best approach here, depending on how one might go about trading volatility derivatives. I suspect that studying the behavior of volatility instruments might warrant some special consideration, but that's beyond the scope of the post. For our purposes here, let's treat the index the way we would any other tradable instrument. The daily "price" differences look like this:

{% chart fig-45-stocks-daily-VIX-diffs.svg %}

The variance seems to be a bit more consistent than that of the price differences of the S&P 500 ETF shown earlier. What does the histogram look like? Not much different than the individual stocks and ETFs examined earlier:

{% chart fig-46-stocks-daily-VIX-probs.svg %}

It looks like the changes in implied volatility vary from one day to the next according to a distribution that might be somewhere between a normal distribution and a Laplace distribution. Here is a histogram of five days worth of one minute intraday values:

{% chart fig-47-stocks-intraday-VIX-probs.svg %}

The intraday data appear to fit the Laplace distribution better than the normal distribution. I was expecting the values of the volatility index to have different characteristics than values from the other data sets, but that doesn't seem to be the case.

## Foreign Exchange

Do the exchange rates between different fiat currencies exhibit the same properties as the data sets studied above? Let's consider the exchange rate between the euro and the US dollar. Here is a chart of the daily exchange rate values over a period of about 18 years:

{% chart fig-48-forex-daily-EURUSD-price.svg %}

As before, we're using the logarithmic values here instead of the real values. Here is a plot of the daily differences in logarithmic exchange rate values:

{% chart fig-49-forex-daily-EURUSD-diffs.svg %}

There does seem to be some heteroskedasticity, but it's not as pronounced as what we saw with the S&P 500 ETF. Here is the histogram:

{% chart fig-50-forex-daily-EURUSD-probs.svg %}

Here we see a familiar pattern. It takes the shape roughly of a Laplace distribution.

## Currency Pairs (Daily)

To see if the pattern holds, let's take a look at the daily exchange rates of a few more currency pairs. Here is the list:

{% latex fig-51 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|>{$}wl{4em}<{$}|>{$}wl{15em}<{$}|}
    \hline
    \text{Symbol}    & \text{Underlying Asset}
    \\[0.25em]\hline
    \texttt{USD/JPY} & \text{US dollar / Japanese yen}
    \\[0.25em]\hline
    \texttt{USD/MXN} & \text{US dollar / Mexican peso}
    \\[0.25em]\hline
    \texttt{USD/RUB} & \text{US dollar / Russian ruble}
    \\[0.25em]\hline
    \texttt{USD/TRY} & \text{US dollar / Turkish lira}
    \\[0.25em]\hline
    \texttt{USD/ZAR} & \text{US dollar / South African rand}
    \\[0.25em]\hline
    \texttt{EUR/NOK} & \text{Euro / Norwegian krone}
    \\[0.25em]\hline
    \texttt{EUR/SEK} & \text{Euro / Swedish krona}
    \\[0.25em]\hline
    \texttt{EUR/TRY} & \text{Euro / Turkish lira}
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here are the charts:

{% chart fig-52-forex-daily-USDJPY-probs.svg %}
{% chart fig-53-forex-daily-USDMXN-probs.svg %}
{% chart fig-54-forex-daily-USDRUB-probs.svg %}
{% chart fig-55-forex-daily-USDTRY-probs.svg %}
{% chart fig-56-forex-daily-USDZAR-probs.svg %}
{% chart fig-57-forex-daily-EURNOK-probs.svg %}
{% chart fig-58-forex-daily-EURSEK-probs.svg %}
{% chart fig-59-forex-daily-EURTRY-probs.svg %}

And here we see the familiar pattern once again.

## Currency Pairs (Intraday)

For the same currency pairs as above, let's examine the distribution of price fluctuations for one minute intraday data over a span of 24 hours:

{% chart fig-60-forex-intraday-USDJPY-probs.svg %}
{% chart fig-61-forex-intraday-USDMXN-probs.svg %}
{% chart fig-62-forex-intraday-USDRUB-probs.svg %}
{% chart fig-63-forex-intraday-USDTRY-probs.svg %}
{% chart fig-64-forex-intraday-USDZAR-probs.svg %}
{% chart fig-65-forex-intraday-EURNOK-probs.svg %}
{% chart fig-66-forex-intraday-EURSEK-probs.svg %}
{% chart fig-67-forex-intraday-EURTRY-probs.svg %}

Notice in some of these charts there is a large spike in the concentration of price changes at or around zero. I suspect this is because there are certain times of day where these instruments are thinly traded and don't move very much.

## Cryptocurrencies

A number of different cryptocurrencies have emerged in the past few years. And some of them have made enormous price moves. Can we expect the price fluctuations of these digital assets to exhibit the same characteristics as stocks and currencies? Let's examine a few of the most popular ones:

{% latex fig-68 %}
    \usepackage{array}
    \setlength{\arraycolsep}{1em}
    \begin{document}
    \begin{displaymath}
    \begin{array}{@{\rule{0em}{1.25em}}|>{$}wl{4em}<{$}|>{$}wl{15em}<{$}|}
    \hline
    \text{Symbol}    & \text{Underlying Asset}
    \\[0.25em]\hline
    \texttt{BTC}     & \text{Bitcoin}
    \\[0.25em]\hline
    \texttt{ETH}     & \text{Ethereum}
    \\[0.25em]\hline
    \texttt{XRP}     & \text{Ripple}
    \\[0.25em]\hline
    \end{array}
    \end{displaymath}
    \end{document}
{% endlatex %}

Here are the charts:

{% chart fig-69-crypto-daily-BTC-probs.svg %}
{% chart fig-70-crypto-daily-ETH-probs.svg %}
{% chart fig-71-crypto-daily-XRP-probs.svg %}

For all three of these data sets, the variation in daily price moves does not conform to the normal distribution. In fact, the distribution of price movements seems to be even more clustered around the center than would be expected for a Laplace distribution.

## Drawing Conclusions

Can we draw any conclusions from this experiment? I think it's safe to say prices fluctuations are not always normally distributed. In all data sets examined, the kurtosis is more leptokurtic than a normal distribution to some degree. But would it be appropriate to use a Laplace distribution to model price movements?

Vance Harwood from *Six Figure Investing* wrote an interesting article asserting that the Laplace distribution should be used instead of the normal distribution to model stock price movements. You can read it here:

* [*Predicting Stock Market Returns---Lose the Normal and Switch to Laplace*](https://sixfigureinvesting.com/2016/03/modeling-stock-market-returns-with-laplace-distribution-instead-of-normal/)

His analysis includes a deeper study on the tails of the distribution. While he acknowledges the observed price data include a higher frequency of large moves than what would be expected from a Laplace distribution, I agree with his assessment that the Laplace distribution is a better alternative compared to the normal distribution.

The *Business Forecasting* blog authored by Clive Jones includes a whole series of articles on the topic of price change distributions:

* [*The Distribution of Daily Stock Market Returns*](http://businessforecastblog.com/the-distribution-of-daily-stock-market-returns/)
* [*The Laplace Distribution and Financial Returns*](http://businessforecastblog.com/the-laplace-distribution-and-financial-returns/)
* [*The Nasdaq 100 Daily Returns and Laplace Distributed Errors*](http://businessforecastblog.com/the-nasdaq-100-daily-returns-and-laplace-distributed-errors/)
* [*Microsoft Stock Prices and the Laplace Distribution*](http://businessforecastblog.com/microsoft-stock-prices-and-the-laplace-distribution/)
* [*Distributions of Stock Returns and Other Asset Prices*](http://businessforecastblog.com/distributions-of-stock-returns-and-other-asset-prices/)

This series cites plenty of evidence that favors the use of the Laplace distribution to model price movements. Nonetheless, as I have illustrated in this post, there are still some cases where the histogram does not exhibit the pointy shaped peak characteristic of the Laplace distribution.

Based on his study of historical cotton prices, Mandelbrot claims that price fluctuations are best described by a family of [stable distributions](https://en.wikipedia.org/wiki/Stable_distribution). These distributions are more rounded at the peak than the Laplace distribution. I currently don't know enough about this class of probability distributions to draw my own conclusions, but I think it's an area worthy of further study.

Another possibility is that the distribution of price fluctuations could take the form of a [generalized normal distribution](https://en.wikipedia.org/wiki/Generalized_normal_distribution). This is a family of distributions that can take the shape of a normal distribution, a Laplace distribution, or something else depending on the value of a shape parameter. I think this might be another area worth further investigation.

{% accompanying_src_link %}
