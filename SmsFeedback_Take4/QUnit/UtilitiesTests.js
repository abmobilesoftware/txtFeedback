"use strict";
$(document).ready(function () {
   module("Utilities");
   //#region cleanupPhoneNumber
   test("cleanupPhoneNumber_passNumberWithLeading+_returnsNumberWithout+", function () {
      var inputNumber = "+40751569435";
      var expectedNumber = "40751569435";
      var res = cleanupPhoneNumber(inputNumber);
      deepEqual(res, expectedNumber, "No leading + accepted");
   });
   test("cleanupPhoneNumber_passNumberWithLeading00_returnsNumberWithout00", function () {
      var inputNumber = "0040751569435";
      var expectedNumber = "40751569435";
      var res = cleanupPhoneNumber(inputNumber);
      deepEqual(res, expectedNumber, "No leading 0 accepted");
   });
   test("cleanupPhoneNumber_passNumberWithMultiple00groups_returnsNumberWithoutLeading00", function () {
      var inputNumber = "0040745009000";
      var expectedNumber = "40745009000";
      var res = cleanupPhoneNumber(inputNumber);
      deepEqual(res, expectedNumber, "Only leading 00 is removed");
   });
   test("cleanupPhoneNumber_passNumberWithoutPrefix_returnsUnchangedNumber", function () {
      var inputNumber = "0745009000";
      var expectedNumber = "0745009000";
      var res = cleanupPhoneNumber(inputNumber);
      deepEqual(res, expectedNumber, "If no prefix no changes are made");
   });
   //#endregion
   //#region comparePhoneNumbers
   test("comparePhoneNumbers_numberWith+PrefixVsnumberWith+Prefix_returnsTrue", function () {
      var inputNumber1 = "+40745009000";
      var inputNumber2 = "+40745009000";
      ok(comparePhoneNumbers(inputNumber1, inputNumber2), "Numbers with same prefix format are equal");      
   });
   test("comparePhoneNumbers_numberWith+PrefixVsnumberWith00Prefix_returnsTrue", function () {
      var inputNumber1 = "0040745009000";
      var inputNumber2 = "+40745009000";
      ok(comparePhoneNumbers(inputNumber1, inputNumber2), "Numbers with different prefix formats are equal");
   });
   test("comparePhoneNumbers_differentNumbersWithoutPrefix_returnsFalse", function () {
      var inputNumber1 = "40745009000";
      var inputNumber2 = "40745009001";
      ok(comparePhoneNumbers(inputNumber1, inputNumber2) === false, "Different numbers are different");
   });
   test("comparePhoneNumbers_differentNumbersWithSamePrefix+_returnsFalse", function () {
      var inputNumber1 = "+40745009000";
      var inputNumber2 = "+40745009001";
      ok(comparePhoneNumbers(inputNumber1, inputNumber2) === false, "Different numbers are different no matter the prefix");
   });
   test("comparePhoneNumbers_differentNumbersWithSamePrefix00_returnsFalse", function () {
      var inputNumber1 = "0040745009000";
      var inputNumber2 = "0040745009001";
      ok(comparePhoneNumbers(inputNumber1, inputNumber2) === false, "Different numbers are different no matter the prefix");
   });
   test("comparePhoneNumbers_differentNumbersWithDifferentPrefix_returnsFalse", function () {
      var inputNumber1 = "+40745009000";
      var inputNumber2 = "0040745009001";
      ok(comparePhoneNumbers(inputNumber1, inputNumber2) === false, "Different numbers are different no matter the prefix");
   });
   //#endregion
   //#region buildConversationID
   test("buildConversationID_validNumbersWithoutPrefix_returnsConcatenationWithSeparator", function () {
      var phFrom = "0745009000";
      var phTo = "0745124125";
      var expectedResult = "0745009000-0745124125";
      deepEqual(buildConversationID(phFrom, phTo), expectedResult, "If no prefixes the conversation Id will be just the concatenation with separator");
   });
   test("buildConversationID_validNumbersWithPrefix00AndNothing_returnsConcatenationWithSeparator", function () {
      var phFrom = "000745009000";
      var phTo = "0745124125";
      var expectedResult = "0745009000-0745124125";
      deepEqual(buildConversationID(phFrom, phTo), expectedResult, "Prefixes are trimmed out of the result");
   });
   test("buildConversationID_validNumbersWithPrefixNothingAnd+_returnsConcatenationWithSeparator", function () {
      var phFrom = "0745009000";
      var phTo = "+0745124125";
      var expectedResult = "0745009000-0745124125";
      deepEqual(buildConversationID(phFrom, phTo), expectedResult, "Prefixes are trimmed out of the result");
   });
   test("buildConversationID_validNumbersWithPrefix+And00_returnsConcatenationWithSeparator", function () {
      var phFrom = "+0745009000";
      var phTo = "000745124125";
      var expectedResult = "0745009000-0745124125";
      deepEqual(buildConversationID(phFrom, phTo), expectedResult, "Prefixes are trimmed out of the result");
   });
   //#endregion
   //#region getFromToFromConversation
   test("getFromToFromConversation_validConversationId_returnsArrayWithFromTo", function () {
      var convId = "0745009000-0745124125";
      var expectedFrom = "0745009000";
      var expectedTo = "0745124125";
      var fromTo = getFromToFromConversation(convId);
      deepEqual(fromTo[0], expectedFrom, "The first position belongs to the from number");
      deepEqual(fromTo[1], expectedTo, "The second position belongs to the to number");
   });
   //#endregion
});