﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.Web.DataContracts;
using System.Configuration;
using GreenField.Web.Helpers;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Calculations for Market Cap Grid
    /// </summary>
    public class MarketCapitalizationCalculations
    {
     
        /// <summary>
        /// Calculates Weighted Average for Portfolio
        /// </summary>
        /// <param name="marketCapDetails"></param>
        /// <returns>_portfolioWeightedAvg</returns>
        public static decimal? CalculatePortfolioWeightedAvg(List<MarketCapitalizationData> marketCapDetails)
        {
            decimal? totalPortfolioMV = 0;
            decimal? portfolioWtdAvg = 0;

            if (marketCapDetails == null)
                throw new ArgumentNullException(GreenfieldConstants.MARKET_CAPITALIZATION);

            //Calculate total Market Value(DirtyValue_PC) for portfolio
            totalPortfolioMV = marketCapDetails.Sum(a => a.PortfolioDirtyValuePC);

            foreach (MarketCapitalizationData _mktCapData in marketCapDetails)
            {
                if (!string.IsNullOrEmpty(_mktCapData.Portfolio_ID))
                    portfolioWtdAvg = portfolioWtdAvg + (_mktCapData.PortfolioDirtyValuePC / totalPortfolioMV * _mktCapData.MarketCapitalInUSD);
            }           

            return portfolioWtdAvg;
        }
        /// <summary>
        /// Calculates Weighted Average for Benchmark
        /// </summary>
        /// <param name="marketCapDetails"></param>
        /// <returns>_benchmarkWeightedAvg</returns>
        public static decimal? CalculateBenchmarkWeightedAvg(List<MarketCapitalizationData> marketCapDetails)
        {
            decimal? totalBenchmarkWeight = 0;
            decimal? benchmarkWtdAvg = 0;

            if (marketCapDetails == null)
                throw new ArgumentNullException(GreenfieldConstants.MARKET_CAPITALIZATION);

            //Calculate total benchmark weight for Benchmark
            totalBenchmarkWeight = marketCapDetails.Sum(a => a.BenchmarkWeight);

            foreach (MarketCapitalizationData _mktCapData in marketCapDetails)
            {
                if (string.IsNullOrEmpty(_mktCapData.Portfolio_ID))
                    benchmarkWtdAvg = benchmarkWtdAvg + (_mktCapData.BenchmarkWeight / totalBenchmarkWeight * _mktCapData.MarketCapitalInUSD);
            }
            return benchmarkWtdAvg;
        }
        
        /// <summary>
        /// Calculates Weighted Median for Portfolio
        /// </summary>
        /// <param name="marketCapitalizationData"></param>
        /// <returns>_portfolioWeightdMedian</returns>

        public static decimal? CalculatePortfolioWeightedMedian(List<MarketCapitalizationData> marketCapitalizationData)
        {
            List<MarketCapitalizationData> mktCapDetails = new List<MarketCapitalizationData>();
            decimal? portfolioWeightedMedian = 0;
            decimal? fiftyPercTotalMV = 0;
            decimal? totalPortfolioMV = 0;

            if (marketCapitalizationData == null)
                throw new ArgumentNullException(GreenfieldConstants.MARKET_CAPITALIZATION);

            //Arrange list in ascending order by "MarketCapitalInUSD"
            mktCapDetails = marketCapitalizationData
                                        .Where(list => list.SecurityThemeCode != GreenfieldConstants.CASH)
                                        .OrderBy(list => list.MarketCapitalInUSD).ToList();

            //Calculate total Market Value(DirtyValue_PC) for portfolio
            totalPortfolioMV = mktCapDetails.Sum(a => a.PortfolioDirtyValuePC);

            //Calculate fifty percent of total market value(dirty value ex-cash)
            foreach (MarketCapitalizationData _mktCap in marketCapitalizationData)
            {
                if (!string.IsNullOrEmpty(_mktCap.Portfolio_ID))
                    fiftyPercTotalMV = fiftyPercTotalMV + (_mktCap.PortfolioDirtyValuePC / totalPortfolioMV);
            }
            fiftyPercTotalMV = fiftyPercTotalMV / 2;

            for (int _index = 0; _index < marketCapitalizationData.Count; _index++)
            {
                if (marketCapitalizationData[_index].MarketCapitalInUSD == fiftyPercTotalMV)
                    portfolioWeightedMedian = marketCapitalizationData[_index].MarketCapitalInUSD;
                else if (marketCapitalizationData[_index].MarketCapitalInUSD > fiftyPercTotalMV)
                    portfolioWeightedMedian = (marketCapitalizationData[_index - 1].MarketCapitalInUSD + marketCapitalizationData[_index].MarketCapitalInUSD) / 2;
                else
                    throw new Exception(GreenfieldConstants.INVALID_DATA_ERR_MSG);
            }
            return portfolioWeightedMedian;
        }
        /// <summary>
        /// Calculates Weighted Median for Benchmark
        /// </summary>
        /// <param name="marketCapitalizationData"></param>
        /// <returns>_BenchmarkWeightdMedian</returns>

        public static decimal? CalculateBenchmarkWeightedMedian(List<MarketCapitalizationData> marketCapitalizationData)
        {
            List<MarketCapitalizationData> mktCapDetails = new List<MarketCapitalizationData>();
            decimal? fiftyPercTotalBenchWt = 0;
            decimal? benchmarkWtdMedian = 0;
            decimal? totalBenchmarkWt;

            if (marketCapitalizationData == null)
                throw new ArgumentNullException(GreenfieldConstants.MARKET_CAPITALIZATION);

            //Arrange list in ascending order by "MarketCapitalInUSD"
            mktCapDetails = marketCapitalizationData
                                        .Where(list => list.SecurityThemeCode !=GreenfieldConstants.CASH)
                                        .OrderBy(list => list.MarketCapitalInUSD).ToList();

            //Calculate total Benchmark weight(Ex-Cash)
            totalBenchmarkWt = mktCapDetails.Sum(a => a.BenchmarkWeight);

            //Calculate fifty percent of total market value(dirty value ex-cash)            
            foreach (MarketCapitalizationData _mktCap in marketCapitalizationData)
            {
                if (string.IsNullOrEmpty(_mktCap.Portfolio_ID))
                    fiftyPercTotalBenchWt = fiftyPercTotalBenchWt + (_mktCap.BenchmarkWeight / totalBenchmarkWt);
            }

            fiftyPercTotalBenchWt = fiftyPercTotalBenchWt / 2;

            for (int _index = 0; _index < marketCapitalizationData.Count; _index++)
            {
                if (marketCapitalizationData[_index].MarketCapitalInUSD == fiftyPercTotalBenchWt)
                    benchmarkWtdMedian = marketCapitalizationData[_index].MarketCapitalInUSD;
                else if (marketCapitalizationData[_index].MarketCapitalInUSD > fiftyPercTotalBenchWt)
                    benchmarkWtdMedian = (marketCapitalizationData[_index - 1].MarketCapitalInUSD + marketCapitalizationData[_index].MarketCapitalInUSD) / 2;
                else
                    throw new Exception(GreenfieldConstants.INVALID_DATA_ERR_MSG);
            }
            return benchmarkWtdMedian;
        }

        /// <summary>
        /// To Calculate Sum of weights depending upon the value of MarketCapitalInUSD for Portfolio
        /// </summary>
        /// <param name="marketCapiltalizationData"></param>
        /// <param name="prtRangeForUndefinedMktCap"></param>
        /// <param name="prtRangeForMicroMktCap"></param>
        /// <param name="prtRangeForSmallMktCap"></param>
        /// <param name="prtRangeForMediumMktCap"></param>
        /// <param name="prtRangeForLargeMktCap"></param>
        /// <param name="prtRangeForMegaMktCap"></param>
        /// <returns>_portfolioSumRange</returns>
        public static List<MarketCapitalizationData> CalculateSumPortfolioRanges(List<MarketCapitalizationData> marketCapiltalizationData)//, ref decimal? prtRangeForUndefinedMktCap, ref decimal? prtRangeForMicroMktCap, ref decimal? prtRangeForSmallMktCap, ref decimal? prtRangeForMediumMktCap, ref decimal? prtRangeForLargeMktCap, ref decimal? prtRangeForMegaMktCap)
        {
            List<MarketCapitalizationData> portfolioSumRange = new List<MarketCapitalizationData>();

            //Calculate total Market Value(DirtyValue_PC) for portfolio
            decimal? mktCapTotalDirtyValue = marketCapiltalizationData.Sum(a => a.PortfolioDirtyValuePC);

            //Getting the lower and upper limit's values for all the Ranges.
            //These values are same for Portfolio and Benchmark. Hence, We are taking these values from Portfolio list not from Benchmark
            List<MarketCapitalizationData> mktCapRanges = GetRangeLimit();
            portfolioSumRange[0].LargeRange = mktCapRanges[0].LargeRange;
            portfolioSumRange[0].MediumRange = mktCapRanges[0].MediumRange;
            portfolioSumRange[0].SmallRange = mktCapRanges[0].SmallRange;
            portfolioSumRange[0].MicroRange = mktCapRanges[0].MicroRange;
            portfolioSumRange[0].UndefinedRange = mktCapRanges[0].UndefinedRange;

            //Calcualting sum for different ranges
            foreach (MarketCapitalizationData mktCap in marketCapiltalizationData)
            {
                if (!string.IsNullOrEmpty(mktCap.Portfolio_ID))
                {
                    if (mktCap.MarketCapitalInUSD > mktCapRanges[0].LargeRange)
                        portfolioSumRange[0].PortfolioSumMegaRange = portfolioSumRange[0].PortfolioSumMegaRange + (mktCap.PortfolioDirtyValuePC / mktCapTotalDirtyValue);

                    else if (mktCap.MarketCapitalInUSD >= mktCapRanges[0].MediumRange && mktCap.MarketCapitalInUSD <= mktCapRanges[0].LargeRange)
                        portfolioSumRange[0].PortfolioSumLargeRange = portfolioSumRange[0].PortfolioSumLargeRange + (mktCap.PortfolioDirtyValuePC / mktCapTotalDirtyValue);

                    else if (mktCap.MarketCapitalInUSD >= mktCapRanges[0].SmallRange && mktCap.MarketCapitalInUSD < mktCapRanges[0].MediumRange)
                        portfolioSumRange[0].PortfolioSumMediumRange = portfolioSumRange[0].PortfolioSumMediumRange + (mktCap.PortfolioDirtyValuePC / mktCapTotalDirtyValue);

                    else if (mktCap.MarketCapitalInUSD >= mktCapRanges[0].MicroRange && mktCap.MarketCapitalInUSD < mktCapRanges[0].SmallRange)
                        portfolioSumRange[0].PortfolioSumSmallRange = portfolioSumRange[0].PortfolioSumSmallRange + (mktCap.PortfolioDirtyValuePC / mktCapTotalDirtyValue);

                    else if (mktCap.MarketCapitalInUSD > mktCapRanges[0].UndefinedRange && mktCap.MarketCapitalInUSD < mktCapRanges[0].MicroRange)
                        portfolioSumRange[0].PortfolioSumMicroRange = portfolioSumRange[0].PortfolioSumMicroRange + (mktCap.PortfolioDirtyValuePC / mktCapTotalDirtyValue);

                    else if (mktCap.MarketCapitalInUSD == mktCapRanges[0].UndefinedRange)
                        portfolioSumRange[0].PortfolioSumUndefinedRange = portfolioSumRange[0].PortfolioSumUndefinedRange + (mktCap.PortfolioDirtyValuePC / mktCapTotalDirtyValue);
                }
            }
            return portfolioSumRange;
        }
        /// <summary>
        ///  To Calculate Sum of weights depending upon the value of MarketCapitalInUSD for Benchmark
        /// </summary>
        /// <param name="marketCapiltalizationData"></param>
        /// <returns>_benchmarkSumRange</returns>
        public static List<MarketCapitalizationData> CalculateSumBenchmarkRanges(List<MarketCapitalizationData> marketCapiltalizationData)//, ref decimal? prtRangeForUndefinedMktCap, ref decimal? prtRangeForMicroMktCap, ref decimal? prtRangeForSmallMktCap, ref decimal? prtRangeForMediumMktCap, ref decimal? prtRangeForLargeMktCap, ref decimal? prtRangeForMegaMktCap)
        {
            List<MarketCapitalizationData> benchmarkSumRange = new List<MarketCapitalizationData>();

            //Calculate total Market Value(DirtyValue_PC) for portfolio
            decimal? mktCapTotalBenchmarkWt = marketCapiltalizationData.Sum(a => a.BenchmarkWeight);

            //Getting the lower and upper limit's values for all the Ranges
            List<MarketCapitalizationData> mktCapRanges = GetRangeLimit();

            ////No need to take these values again as it already from Portfolio list in the above method

            //_benchmarkSumRange[0].LargeRange = _mktCapRanges[0].LargeRange;
            //_benchmarkSumRange[0].MediumRange = _mktCapRanges[0].MediumRange;
            //_benchmarkSumRange[0].SmallRange = _mktCapRanges[0].SmallRange;
            //_benchmarkSumRange[0].MicroRange = _mktCapRanges[0].MicroRange;
            //_benchmarkSumRange[0].UndefinedRange = _mktCapRanges[0].UndefinedRange;

            //Calcualting sum for different ranges
            foreach (MarketCapitalizationData mktCapData in marketCapiltalizationData)
            {
                if (string.IsNullOrEmpty(mktCapData.Portfolio_ID))
                {
                    if (mktCapData.MarketCapitalInUSD > mktCapRanges[0].LargeRange)
                        benchmarkSumRange[0].BenchmarkSumMegaRange = benchmarkSumRange[0].BenchmarkSumMegaRange + (mktCapData.BenchmarkWeight / mktCapTotalBenchmarkWt);

                    else if (mktCapData.MarketCapitalInUSD >= mktCapRanges[0].MediumRange && mktCapData.MarketCapitalInUSD <= mktCapRanges[0].LargeRange)
                        benchmarkSumRange[0].BenchmarkSumLargeRange = benchmarkSumRange[0].BenchmarkSumLargeRange + (mktCapData.BenchmarkWeight / mktCapTotalBenchmarkWt);

                    else if (mktCapData.MarketCapitalInUSD >= mktCapRanges[0].SmallRange && mktCapData.MarketCapitalInUSD < mktCapRanges[0].MediumRange)
                        benchmarkSumRange[0].BenchmarkSumMediumRange = benchmarkSumRange[0].BenchmarkSumMediumRange + (mktCapData.BenchmarkWeight / mktCapTotalBenchmarkWt);

                    else if (mktCapData.MarketCapitalInUSD >= mktCapRanges[0].MicroRange && mktCapData.MarketCapitalInUSD < mktCapRanges[0].SmallRange)
                        benchmarkSumRange[0].BenchmarkSumSmallRange = benchmarkSumRange[0].BenchmarkSumSmallRange + (mktCapData.BenchmarkWeight / mktCapTotalBenchmarkWt);

                    else if (mktCapData.MarketCapitalInUSD > mktCapRanges[0].UndefinedRange && mktCapData.MarketCapitalInUSD < mktCapRanges[0].MicroRange)
                        benchmarkSumRange[0].BenchmarkSumMicroRange = benchmarkSumRange[0].BenchmarkSumMicroRange + (mktCapData.BenchmarkWeight / mktCapTotalBenchmarkWt);

                    else if (mktCapData.MarketCapitalInUSD == mktCapRanges[0].UndefinedRange)
                        benchmarkSumRange[0].BenchmarkSumUndefinedRange = benchmarkSumRange[0].BenchmarkSumUndefinedRange + (mktCapData.BenchmarkWeight / mktCapTotalBenchmarkWt);
                }
            }
            return benchmarkSumRange;
        }

        /// <summary>
        /// Get limit values for all ranges from web.config
        /// </summary>
        /// <returns>_rangeLimit</returns>
        public static List<MarketCapitalizationData> GetRangeLimit()
        {

            List<MarketCapitalizationData> _rangeLimit = new List<MarketCapitalizationData>();
            MarketCapitalizationData _mktCapData = new MarketCapitalizationData();

            string[] _undefRanges = GreenfieldConstants.UNDEFINED_RANGE.Split(',');

            //Check if range value is null or blank then consider it as UndefinedRange
            if (_undefRanges.Contains(ConfigurationManager.AppSettings[GreenfieldConstants.NULL_VAL]) || _undefRanges.Contains(ConfigurationManager.AppSettings[GreenfieldConstants.BLANK_VAL]) || _undefRanges.Contains(ConfigurationManager.AppSettings[GreenfieldConstants.DECIMAL_DEF_VAL]))
                _mktCapData.UndefinedRange =  Convert.ToDecimal(GreenfieldConstants.DECIMAL_DEF_VAL);

            _mktCapData.LargeRange = Convert.ToDecimal(ConfigurationManager.AppSettings[GreenfieldConstants.LARGE_RANGE]);
            _mktCapData.MediumRange = Convert.ToDecimal(ConfigurationManager.AppSettings[GreenfieldConstants.MEDIUM_RANGE]);
            _mktCapData.SmallRange = Convert.ToDecimal(ConfigurationManager.AppSettings[GreenfieldConstants.SMALL_RANGE]);
            _mktCapData.MicroRange = Convert.ToDecimal(ConfigurationManager.AppSettings[GreenfieldConstants.MICRO_RANGE]);
            
            _rangeLimit.Add(_mktCapData);
            return _rangeLimit;
        }

    }
}