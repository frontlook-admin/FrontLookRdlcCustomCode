/**
 * FL_NumberToStrings - Convert numbers to words (Indian numbering system)
 * Supports: number (double), integer, BigInt
 * @version 3.0
 */

const FL_NumberToStrings = (() => {
    // Currency array: [Name, Decimal Name, Symbol, Format]
    const currencyDenotionIndian = ["Rupees", "Paise", "₹", "#,##,##0.00"];

    // Cached arrays to avoid recreation
    const unitsMap = ["Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
        "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
        "Seventeen", "Eighteen", "Nineteen"];
    const tensMap = ["Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"];

    /**
     * Format number as currency with symbol and proper formatting
     * @param {number} number - The number to format
     * @param {Array<string>} currencyDenotion - Optional: Currency array [Name, Decimal Name, Symbol, Format]
     * @returns {string} Formatted currency string
     */
    function formatCurrency(number, currencyDenotion = null) {
        const currency = currencyDenotion && currencyDenotion.length >= 4 ? currencyDenotion : currencyDenotionIndian;
        const symbol = currency[2];
        const format = currency[3];

        // Parse format string and apply Indian numbering
        let formatted;
        if (format === "#,##,##0.00") {
            // Indian format: 12,34,567.89
            formatted = formatIndianNumbering(number);
        } else if (format === "#,##0.00") {
            // Western format: 1,234,567.89
            formatted = number.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        } else {
            // Default to 2 decimal places
            formatted = number.toFixed(2);
        }

        return `${symbol}${formatted}`;
    }

    /**
     * Format number with Indian numbering system
     * @param {number} number - Number to format
     * @returns {string} Formatted number
     */
    function formatIndianNumbering(number) {
        const parts = number.toFixed(2).split('.');
        const intPart = parts[0];
        const decPart = parts[1];

        // Handle negative numbers
        const isNegative = intPart.startsWith('-');
        const absIntPart = isNegative ? intPart.substring(1) : intPart;

        let formatted = '';
        const len = absIntPart.length;

        if (len <= 3) {
            formatted = absIntPart;
        } else {
            // Last 3 digits
            formatted = absIntPart.substring(len - 3);
            let remaining = absIntPart.substring(0, len - 3);

            // Add groups of 2 digits
            while (remaining.length > 0) {
                if (remaining.length <= 2) {
                    formatted = remaining + ',' + formatted;
                    break;
                } else {
                    formatted = remaining.substring(remaining.length - 2) + ',' + formatted;
                    remaining = remaining.substring(0, remaining.length - 2);
                }
            }
        }

        return (isNegative ? '-' : '') + formatted + '.' + decPart;
    }

    /**
     * Convert number to words with optional currency formatting
     * @param {number|string} number - The number to convert
     * @param {boolean} ifCurrency - Format as currency
     * @param {boolean} showCurrency - Show currency denomination
     * @param {string|Array<string>} currencyDenotion - Custom currency (format: "Major;Minor;Symbol;Format" or array)
     * @returns {string} Number in words
     */
    function toWordsIn(number, ifCurrency = true, showCurrency = true, currencyDenotion = null) {
        // Input validation
        if (number === null || number === undefined || number === '') {
            return "Zero";
        }

        const numStr = typeof number === 'number' ? number.toString() : String(number);
        const num = numStr.split('.');

        // Parse integer part
        const intPart = parseInt(num[0], 10);
        if (isNaN(intPart)) {
            return "Zero";
        }

        let words1 = toWordsInLong(intPart);
        let word2 = '';

        // Parse decimal part
        if (num.length > 1) {
            const decPart = parseInt(num[1], 10);
            if (!isNaN(decPart) && decPart > 0) {
                word2 = toWordsInAfterPoint(num[1], ifCurrency);
            }
        }

        if (ifCurrency) {
            if (showCurrency) {
                let currencyArray;
                if (typeof currencyDenotion === 'string') {
                    const _curDenotion = currencyDenotion && currencyDenotion.split(';').length === 4
                        ? currencyDenotion.split(';')
                        : null;
                    currencyArray = _curDenotion && _curDenotion.length === 4
                        ? _curDenotion
                        : currencyDenotionIndian;
                } else {
                    currencyArray = currencyDenotion && currencyDenotion.length >= 4
                        ? currencyDenotion
                        : currencyDenotionIndian;
                }

                return word2
                    ? `${currencyArray[0]} ${words1.replace(/ and /g, " ")} And ${word2} ${currencyArray[1]} Only`
                    : `${currencyArray[0]} ${words1} Only`;
            }
            return word2
                ? `${words1.replace(/ and /g, " ")} And ${word2} ${currencyDenotionIndian[1]} Only`
                : `${words1} Only`;
        }

        return word2 ? `${words1} Point ${word2}` : words1;
    }

    /**
     * Convert long/integer to words
     * @param {number} number - Integer to convert
     * @returns {string} Number in words
     */
    function toWordsInLong(number) {
        if (number === 0) return "Zero";
        if (number < 0) return "Minus " + toWordsInLong(Math.abs(number));

        let words = "";
        let remaining = number;

        // Crore (10,000,000)
        if (remaining >= 10000000) {
            words += toWordsInLong(Math.floor(remaining / 10000000)) + " Crore ";
            remaining %= 10000000;
        }
        // Lakh (100,000)
        if (remaining >= 100000) {
            words += toWordsInLong(Math.floor(remaining / 100000)) + " Lakh ";
            remaining %= 100000;
        }
        // Thousand (1,000)
        if (remaining >= 1000) {
            words += toWordsInLong(Math.floor(remaining / 1000)) + " Thousand ";
            remaining %= 1000;
        }
        // Hundred (100)
        if (remaining >= 100) {
            words += toWordsInLong(Math.floor(remaining / 100)) + " Hundred ";
            remaining %= 100;
        }

        if (remaining <= 0) return words.trim();
        if (words !== "") words += "and ";

        if (remaining < 20) {
            words += unitsMap[remaining];
        } else {
            words += tensMap[Math.floor(remaining / 10)];
            const ones = remaining % 10;
            if (ones > 0) {
                words += "-" + unitsMap[ones];
            }
        }

        return words;
    }

    /**
     * Convert decimal part to words
     * @param {string} number - Decimal part as string
     * @param {boolean} ifCurrency - Is currency format
     * @returns {string} Decimal part in words
     */
    function toWordsInAfterPoint(number, ifCurrency) {
        if (!number || number === '0') {
            return '';
        }

        const parsed = parseInt(number, 10);
        if (isNaN(parsed) || parsed <= 0) {
            return '';
        }

        if (ifCurrency) {
            // Pad to 2 digits for currency (paise/cents)
            const padded = number.length === 1 ? number + "0" : number.substring(0, 2);
            return toWordsInLong(parseInt(padded, 10));
        } else {
            // Spell out each digit for non-currency decimals
            const digits = number.split('');
            const words = digits.map(digit => {
                const d = parseInt(digit, 10);
                return isNaN(d) ? '' : unitsMap[d];
            }).filter(w => w !== '');

            return words.join(' ');
        }
    }

    /**
     * Convert number to minimized format (e.g., 1.5 Lakh)
     * @param {number} number - Number to minimize
     * @returns {string} Minimized representation
     */
    function toWordsMinimised(number) {
        // Input validation
        if (typeof number !== 'number' || isNaN(number)) {
            return "0";
        }

        const absNumber = Math.abs(number);

        if (absNumber < 100) {
            return number.toString();
        }

        let unit = "";
        let divider = 100;

        if (absNumber >= 10000000) {
            divider = 10000000;
            unit = "Crore";
        } else if (absNumber >= 100000) {
            divider = 100000;
            unit = "Lakh";
        } else if (absNumber >= 1000) {
            divider = 1000;
            unit = "Thousand";
        } else { // >= 100
            divider = 100;
            unit = "Hundred";
        }

        const result = number / divider;
        const formatted = Math.floor(Math.abs(result)) === Math.abs(result)
            ? result.toFixed(0)
            : result.toFixed(1);

        return `${formatted} ${unit}`;
    }

    // Public API
    return {
        toWordsIn,
        toWordsInLong,
        toWordsMinimised,
        toWordsInAfterPoint,
        formatCurrency,
        formatIndianNumbering,
        currencyDenotionIndian
    };
})();

// Export for Node.js/CommonJS
if (typeof module !== 'undefined' && module.exports) {
    module.exports = FL_NumberToStrings;
}

// Export for ES6 modules
if (typeof exports !== 'undefined') {
    exports.FL_NumberToStrings = FL_NumberToStrings;
}
