/*! utils-inputsearch - v1.0.1 - 2015-01-23
* Copyright (c) 2015 Advisory Board Company */

/*global _, Modernizr */

/**
A HTML5 polyfill for input[type="search"]. Adds a custom cancel button to the input search field in the browsers that do not have support for the cancel button.
The cancel button allows to clear the input field. This feature is currently supported only in webkit browsers.
@class InputSearch
@module Widgets

@demo docs/demos/inputsearch.jade
**/
(function (window, define, factory, undefined) {
    'use strict';

    if (define !== undefined) {
        define(['./cs.modernizr.js'],factory);
    } else {
        factory(Modernizr);
    }
}(this, this.define, function (modernizr) {
  "use strict";

  /**
   * Searchfield constructor
   *
   * @param {jQuery|Element} $el Input element we want to augment
   * @param {Object=} options Behaviour options
   * @constructor
   */
  function Searchfield($el, options){
    this.$el = $el;
    this.options = options || {};
    this.$cancelButton = null;

    if (!$el || !$el instanceof $){
      throw new TypeError('$el should be a jQuery Selector instance.');
    }

    // Initializing features
    this.options.showCancel && this.setupCancelButton();
  }

  // Methods
  Searchfield.prototype = {
    /**
     * Clear the search field
     * @method clear
     */
    clear: function clear(){
      this.$el.val('');

      this.options.focusAfterClear;
      this.hideCancelButton();
    },
    /**
     * Creates a Cancel Button and attach events to it
     * @method setupCancelButton
     */
    setupCancelButton: function setupCancelButton(){
      this.$cancelButton = $( document.createElement('div') );

      this.$cancelButton
        .addClass('search-cancel-button hidden')
        .on('click', $.proxy(this.clear, this) )
        .insertAfter(this.$el);
    },
    /**
     * Positions the Cancel Button to where it belongs
     * @method repositionCancelButton
     */
    repositionCancelButton: function repositionCancelButton(){
      var $el = this.$el;
      var position = $el.offset();
      var previousPosition = $el.data('input-search-position') || {};

      // No need to recalculate position
      if (previousPosition.left === position.left && previousPosition.top === position.top){
          return;
      }
      else{
          $el.data('input-search-position', $.extend({}, position));
      }

      position.left += $el.outerWidth() - this.options.offsetRight - this.$cancelButton.outerWidth() - (parseInt($el.css('border-right'), 10) || 0);

      //simulating top=50% + margin-top=-halfsize for middle vertical align
      position.top += (($el.innerHeight() / 2) + (parseInt($el.css('border-top-width'), 10) || 0)) + this.options.offsetTop;
      position.top -= this.$cancelButton.outerHeight() / 2;

      this.$cancelButton.offset(position);
    },
    /**
     * Hide the Cancel Button when the field is empty
     * @method maybeHideCancelButton
     */
    maybeHideCancelButton: function maybeHideCancelButton(){
      var isVisible = !this.$cancelButton.hasClass('hidden'),
          inpVal = $.trim(this.$el.val());

      if(inpVal.length === 0) {
        this.hideCancelButton();
      }
      else {
        this.showCancelButton();
      }
    },
    /**
     * Hide the Cancel Button in the searchfield
     * @method hideCancelButton     
     */
    hideCancelButton: function hideCancelButton(){
      this.$cancelButton.addClass('hidden');
    },
    /**
     * Show the Cancel Button in the searchfield
     * @method showCancelButton     
     */
    showCancelButton: function showCancelButton(){
      this.repositionCancelButton();
      this.$cancelButton.css('visibility', '').removeClass('hidden');
    }
  };

  /**
   * jQuery Plugin for Searchfield
   *
   * @param {Object|String|undefined=} option
   */
  $.fn.inputSearch = function inputSearch(option){
    var defaults = {
        /**
          If set to true (by default), the related input element will receive back the focus after the user has clicked on the cancel button.

          @property focusAfterClear
          @type Boolean

          @default true
        **/
        focusAfterClear: true,

        /**
          Option to choose how many display pixels you need as a righ gap between the cancel button and the rightmost boundary of the input element.
    
          @property offsetRight
          @type Number

          @default 10
        **/
        offsetRight: 10,

        /**
          Option to choose how many display pixels you need as a top gap between the cancel button and the topmost boundary of the input element.
    
          @property offsetTop
          @type Number

          @default 0
        **/
        offsetTop: 0,

        /**
          If set to true (by default), will display a cancel button after the input has been focused. The button will appear only if the input element has a non-empty value.
    
          @property showCancel
          @type Boolean

          @default true
        **/
        showCancel: true
    },
    options = $.extend({}, defaults, typeof option === 'object' && option);

    return $(this).each(function(){
      var $input = $(this);
      var data = $input.data('input-search');

      if (!data){
        $input.data('input-search', (data = new Searchfield($input, options)));
      }

      if (typeof option === 'string'){
        data[option]();
      }
    });
  };

  /**
   * @type {Searchfield}
   */
  if(!modernizr.searchcancel) {
    $.fn.inputSearch.Constructor = Searchfield;
  }

  /**
   * Default Event Listeners
   */
  function _initSearchInput(event){
    var inpVal = $.trim($(this).val()).length,
        isfocussed = $(this).is(':focus');

    if(event.type === "focusout" || event.keyCode === 9) {
      $(this).inputSearch('hideCancelButton');
    }
    else if(inpVal > 0 && event.type === "mouseenter" && event.relatedTarget.className !== 'search-cancel-button') {
      $(this).inputSearch('showCancelButton');
    }
    else if(!isfocussed && event.type === "mouseleave" && event.relatedTarget.className !== 'search-cancel-button'){
      $(this).inputSearch('hideCancelButton');
    }
    else if(event.type === undefined) {
      $(this).inputSearch('clear');
    }
    else {
      $(this).inputSearch('maybeHideCancelButton');
    }
  }

  $(document)
    .on('focus blur keyup mouseenter mouseleave', '.no-searchcancel input[type="search"]', _initSearchInput)
    .ready(function(){
        $(this)
        .find('.no-searchcancel input[type="search"][value!=""]')
        .each(_initSearchInput);
    });
}));
