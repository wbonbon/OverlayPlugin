'use strict';

// Map of language -> targetType -> settings title.
const configTitles = {
  English: {
    Target: 'Target Settings',
    Focus: 'Focus Target Settings',
    Hover: 'Hover Target Settings',
    TargetOfTarget: 'Target of Target Settings',
  },
};

const helpText = {
  English: '(🔒lock overlay to hide settings)',
};

// language -> displayed option text -> text key
const textOptions = {
  English: {
    'None': 'None',
    'Current HP': 'CurrentHP',
    'Max HP': 'MaxHP',
    'Current / Max HP': 'CurrentAndMaxHP',
    'Percent HP': 'PercentHP',
    'Distance': 'Distance',
  },
};

const configStructure = [
  {
    id: 'leftText',
    name: {
      English: 'Left Text',
    },
    type: 'select',
    options: textOptions,
    default: 'CurrentAndMaxHP',
  },
  {
    id: 'middleText',
    name: {
      English: 'Middle Text',
    },
    options: textOptions,
    type: 'select',
    default: 'Distance',
  },
  {
    id: 'rightText',
    name: {
      English: 'Right Text',
    },
    options: textOptions,
    type: 'select',
    default: 'PercentHP',
  },
  {
    id: 'barHeight',
    name: {
      English: 'Height of the bar',
    },
    type: 'text',
    default: 11,
  },
  {
    id: 'barWidth',
    name: {
      English: 'Width of the bar',
    },
    type: 'text',
    default: 250,
  },
  {
    id: 'isRounded',
    name: {
      English: 'Enable rounded corners',
    },
    type: 'checkbox',
    default: true,
  },
  {
    id: 'borderSize',
    name: {
      English: 'Size of the border',
    },
    type: 'text',
    default: 1,
  },
  {
    id: 'borderColor',
    name: {
      English: 'Color of the border',
    },
    type: 'text',
    default: 'black',
  },
  {
    id: 'fontSize',
    name: {
      English: 'Size of the font',
    },
    type: 'text',
    default: 10,
  },
  {
    id: 'fontFamily',
    name: {
      English: 'Name of the font',
    },
    type: 'text',
    default: 'Meiryo',
  },
  {
    id: 'fontColor',
    name: {
      English: 'Color of the font',
    },
    type: 'text',
    default: 'white',
  },
  {
    id: 'bgColor',
    name: {
      English: 'Background depleted bar color',
    },
    type: 'text',
    default: 'rgb(4, 15, 4)',
  },
  {
    id: 'fgColorHigh',
    name: {
      English: 'Bar color when hp is high',
    },
    type: 'text',
    default: 'rgb(0, 159, 1)',
  },
  {
    id: 'midColorPercent',
    name: {
      English: 'Percent below where hp is mid',
    },
    type: 'text',
    default: 60,
  },
  {
    id: 'fgColorMid',
    name: {
      English: 'Bar color when hp is mid',
    },
    type: 'text',
    default: 'rgb(160, 130, 30)',
  },
  {
    id: 'lowColorPercent',
    name: {
      English: 'Percent below where hp is mid',
    },
    type: 'text',
    default: 30,
  },
  {
    id: 'fgColorLow',
    name: {
      English: 'Bar color when hp is low',
    },
    type: 'text',
    default: 'rgb(240, 40, 30)',
  },
];

const overlayDataKey = 'targetbars';
const targets = ['Target', 'Focus', 'Hover', 'TargetOfTarget'];
const rawKeys = ['CurrentHP', 'MaxHP', 'Distance'];
const comboKeys = ['PercentHP', 'CurrentAndMaxHP'];
const allKeys = rawKeys.concat(comboKeys);

// Return "str px" if "str" is a number, otherwise "str".
let defaultAsPx = (str) => {
  if (parseFloat(str) == str)
    return str + 'px';
  return str;
};

class BarUI {
  constructor(targetType, topLevelOptions, div) {
    this.target = targetType;
    this.options = topLevelOptions[targetType];
    this.div = div;
    this.lastData = {};
    this.isExampleShowcase = false;

    // Map of keys to elements that contain those values.
    // built from this.options.elements.
    this.elementMap = {};

    const textMap = {
      left: this.options.leftText,
      center: this.options.middleText,
      right: this.options.rightText,
    };
    for (const justifyKey in textMap) {
      let text = textMap[justifyKey];
      let textDiv = document.createElement('div');
      textDiv.classList.add(text);
      textDiv.style.justifySelf = justifyKey;
      this.div.appendChild(textDiv);
      this.elementMap[text] = this.elementMap[text] || [];
      this.elementMap[text].push(textDiv);
    }

    if (this.options.isRounded)
      this.div.classList.add('rounded');
    else
      this.div.classList.remove('rounded');

    // TODO: could move settings container down by height of bar
    // but up to some maximum so it's not hidden if you type in
    // a ridiculous number, vs the absolute position it is now.
    this.div.style.height = defaultAsPx(this.options.barHeight);
    this.div.style.width = defaultAsPx(this.options.barWidth);

    let borderStyle = defaultAsPx(this.options.borderSize);
    borderStyle += ' solid ' + this.options.borderColor;
    this.div.style.border = borderStyle;

    this.div.style.fontSize = defaultAsPx(this.options.fontSize);
    this.div.style.fontFamily = this.options.fontFamily;
    this.div.style.color = this.options.fontColor;

    // Alignment hack:
    // align-self:center doesn't work when children are taller than parents.
    // TODO: is there some better way to do this?
    const containerHeight = parseInt(this.div.clientHeight);
    for (const el in this.elementMap) {
      for (let div of this.elementMap[el]) {
        // Add some text to give div a non-zero height.
        div.innerText = 'XXX';
        let divHeight = div.clientHeight;
        div.innerText = '';
        if (divHeight <= containerHeight)
          continue;
        div.style.position = 'relative';
        div.style.top = defaultAsPx((containerHeight - divHeight) / 2.0);
      }
    }
  }

  // EnmityTargetData event handler.
  update(e) {
    if (!e)
      return;

    // Don't let the game updates override the example showcase.
    if (this.isExampleShowcase && !e.isExampleShowcase)
      return;
    this.isExampleShowcase = e.isExampleShowcase;

    let data = e[this.target];
    // If there's no target, or if the target is something like a marketboard
    // which has zero HP, then don't show the overlay.
    if (!data || data.MaxHP === 0) {
      this.setVisible(false);
      return;
    }

    for (const key of rawKeys) {
      if (data[key] !== this.lastData[key])
        this.setValue(key, data[key]);
    }

    if (data.CurrentHP !== this.lastData.CurrentHP ||
      data.MaxHP !== this.lastData.MaxHP) {
      const percent = 100 * data.CurrentHP / data.MaxHP;
      const percentStr = percent.toFixed(2) + '%';
      this.setValue('PercentHP', percentStr);
      this.updateGradient(percent);

      this.setValue('CurrentAndMaxHP', data.CurrentHP + ' / ' + data.MaxHP);
    }

    this.lastData = data;
    this.setVisible(true);
  }

  updateGradient(percent) {
    // Find the colors from options, based on current percentage.
    let fgColor;
    if (percent > this.options.midColorPercent)
      fgColor = this.options.fgColorHigh;
    else if (percent > this.options.lowColorPercent)
      fgColor = this.options.fgColorMid;
    else
      fgColor = this.options.fgColorLow;

    // Right-fill with fgcolor up to percent, and then bgcolor after that.
    const bgColor = this.options.bgColor;
    let style = 'linear-gradient(90deg, ' +
      fgColor + ' ' + percent + '%, ' + bgColor + ' ' + percent + '%)';
    this.div.style.background = style;
  }

  setValue(name, value) {
    let nodes = this.elementMap[name];
    if (!nodes)
      return;
    for (let node of nodes)
      node.innerText = value;
  }

  setVisible(isVisible) {
    if (isVisible)
      this.div.classList.remove('hidden');
    else
      this.div.classList.add('hidden');
  }
}

class SettingsUI {
  constructor(targetType, lang, configStructure, savedConfig, settingsDiv, rebuildFunc) {
    this.savedConfig = savedConfig || {};
    this.div = settingsDiv;
    this.rebuildFunc = rebuildFunc;
    this.lang = lang;
    this.target = targetType;

    this.buildUI(settingsDiv, configStructure);

    rebuildFunc(savedConfig);
  }

  // Top level UI builder, builds everything.
  buildUI(container, configStructure) {
    container.appendChild(this.buildHeader());
    container.appendChild(this.buildHelpText());
    for (const opt of configStructure) {
      let buildFunc = {
        checkbox: this.buildCheckbox,
        select: this.buildSelect,
        text: this.buildText,
      }[opt.type];
      if (!buildFunc) {
        console.error('unknown type: ' + JSON.stringify(opt));
        continue;
      }

      buildFunc.bind(this)(container, opt, this.target);
    }
  }

  buildHeader() {
    let div = document.createElement('div');
    const titles = this.translate(configTitles);
    div.innerHTML = titles[this.target];
    div.classList.add('settings-title');
    return div;
  }

  buildHelpText() {
    let div = document.createElement('div');
    div.innerHTML = this.translate(helpText);
    div.classList.add('settings-helptext');
    return div;
  }

  // Code after this point in this class is largely cribbed from cactbot's
  // ui/config/config.js CactbotConfigurator.
  // If this gets used again, maybe it should be abstracted.

  async saveConfigData() {
    await callOverlayHandler({
      call: 'saveData',
      key: overlayDataKey,
      data: this.savedConfig,
    });
    this.rebuildFunc(this.savedConfig);
  }

  // Helper translate function.  Takes in an object with locale keys
  // and returns a single entry based on available translations.
  translate(textObj) {
    if (textObj === null || typeof textObj !== 'object' || !textObj['English'])
      return textObj;
    let t = textObj[this.lang];
    if (t)
      return t;
    return textObj['English'];
  }

  // takes variable args, with the last value being the default value if
  // any key is missing.
  // e.g. (foo, bar, baz, 5) with {foo: { bar: { baz: 3 } } } will return
  // the value 3.  Requires at least two args.
  getOption() {
    let num = arguments.length;
    if (num < 2) {
      console.error('getOption requires at least two args');
      return;
    }

    let defaultValue = arguments[num - 1];
    let objOrValue = this.savedConfig;
    for (let i = 0; i < num - 1; ++i) {
      objOrValue = objOrValue[arguments[i]];
      if (typeof objOrValue === 'undefined')
        return defaultValue;
    }

    return objOrValue;
  }

  // takes variable args, with the last value being the 'value' to set it to
  // e.g. (foo, bar, baz, 3) will set {foo: { bar: { baz: 3 } } }.
  // requires at least two args.
  setOption() {
    let num = arguments.length;
    if (num < 2) {
      console.error('setOption requires at least two args');
      return;
    }

    // Set keys and create default {} if it doesn't exist.
    let obj = this.savedConfig;
    for (let i = 0; i < num - 2; ++i) {
      let arg = arguments[i];
      obj[arg] = obj[arg] || {};
      obj = obj[arg];
    }
    // Set the last key to have the final argument's value.
    obj[arguments[num - 2]] = arguments[num - 1];
    this.saveConfigData();
  }

  buildNameDiv(opt) {
    let div = document.createElement('div');
    div.innerHTML = this.translate(opt.name);
    div.classList.add('option-name');
    return div;
  }

  buildCheckbox(parent, opt, group) {
    let div = document.createElement('div');
    div.classList.add('option-input-container');

    let input = document.createElement('input');
    div.appendChild(input);
    input.type = 'checkbox';
    input.checked = this.getOption(group, opt.id, opt.default);
    input.onchange = () => this.setOption(group, opt.id, input.checked);

    parent.appendChild(this.buildNameDiv(opt));
    parent.appendChild(div);
  }

  // <select> inputs don't work in overlays, so make a fake one.
  buildSelect(parent, opt, group) {
    let div = document.createElement('div');
    div.classList.add('option-input-container');
    div.classList.add('select-container');

    // Build the real select so we have a real input element.
    let input = document.createElement('select');
    input.classList.add('hidden');
    div.appendChild(input);

    let defaultValue = this.getOption(group, opt.id, opt.default);
    input.onchange = () => this.setOption(group, opt.id, input.value);

    let innerOptions = this.translate(opt.options);
    for (let key in innerOptions) {
      let elem = document.createElement('option');
      elem.value = innerOptions[key];
      elem.innerHTML = key;
      if (innerOptions[key] == defaultValue)
        elem.selected = true;
      input.appendChild(elem);
    }

    parent.appendChild(this.buildNameDiv(opt));
    parent.appendChild(div);

    // Now build the fake select.
    let selectedDiv = document.createElement('div');
    selectedDiv.classList.add('select-active');
    selectedDiv.innerHTML = input.options[input.selectedIndex].innerHTML;
    div.appendChild(selectedDiv);

    let items = document.createElement('div');
    items.classList.add('select-items', 'hidden');
    div.appendChild(items);

    selectedDiv.addEventListener('click', (e) => {
      items.classList.toggle('hidden');
    });

    // Popout list of options.
    for (let idx = 0; idx < input.options.length; ++idx) {
      let optionElem = input.options[idx];
      let item = document.createElement('div');
      item.classList.add('select-item');
      item.innerHTML = optionElem.innerHTML;
      items.appendChild(item);

      item.addEventListener('click', (e) => {
        input.selectedIndex = idx;
        input.onchange();
        selectedDiv.innerHTML = item.innerHTML;
        items.classList.toggle('hidden');
        selectedDiv.classList.toggle('select-arrow-active');
      });
    }
  }

  buildText(parent, opt, group, step) {
    let div = document.createElement('div');
    div.classList.add('option-input-container');

    let input = document.createElement('input');
    div.appendChild(input);
    input.type = 'text';
    if (step)
      input.step = step;
    input.value = this.getOption(group, opt.id, opt.default);
    let setFunc = () => this.setOption(group, opt.id, input.value);
    input.onchange = setFunc;
    input.oninput = setFunc;

    parent.appendChild(this.buildNameDiv(opt));
    parent.appendChild(div);
  }
}

function updateOverlayState(e) {
  let settingsContainer = document.getElementById('settings-container');
  if (!settingsContainer)
    return;
  const locked = e.detail.isLocked;
  if (locked) {
    settingsContainer.classList.add('hidden');
    document.body.classList.remove('resize-background');
  } else {
    settingsContainer.classList.remove('hidden');
    document.body.classList.add('resize-background');
  }
  OverlayPluginApi.setAcceptFocus(!locked);
}

function showExample(barUI) {
  barUI.update({
    Target: {
      Name: 'TargetMob',
      CurrentHP: 38300,
      MaxHP: 50000,
      Distance: 12.8,
      EffectiveDistance: 7,
    },
    Focus: {
      Name: 'FocusMob',
      CurrentHP: 8123,
      MaxHP: 29123,
      Distance: 52.7,
      EffectiveDistance: 45,
    },
    Hover: {
      Name: 'HoverMob',
      CurrentHP: 2300,
      MaxHP: 2500,
      Distance: 5.2,
      EffectiveDistance: 1,
    },
    TargetOfTarget: {
      Name: 'TargetOfTargetMob',
      CurrentHP: 15123,
      MaxHP: 32748,
      Distance: 12.6,
      EffectiveDistance: 3,
    },
    isExampleShowcase: true,
  });
}

// This event comes early and doesn't depend on any other state.
// So, add the listener before DOMContentLoaded.
document.addEventListener('onOverlayStateUpdate', updateOverlayState);

window.addEventListener('DOMContentLoaded', async (e) => {
  // Initialize language from OverlayPlugin.
  let lang = 'English';
  const langResult = await window.callOverlayHandler({ call: 'getLanguage' });
  if (langResult && langResult.language)
    lang = langResult.language;

  // Determine the type of target bar by a specially named container.
  let containerDiv;
  let targetType;
  for (const key of targets) {
    containerDiv = document.getElementById('container-' + key.toLowerCase());
    if (containerDiv) {
      targetType = key;
      break;
    }
  }
  if (!containerDiv) {
    console.error('Missing container');
    return;
  }

  // Set option defaults from config.
  let options = {};
  options[targetType] = options[targetType] || {};
  for (const opt of configStructure)
    options[targetType][opt.id] = opt.default;

  // Overwrite options from loaded values.  Options are stored once per target type,
  // so that different targets can be configured differently.
  const loadResult = await window.callOverlayHandler({ call: 'loadData', key: overlayDataKey });
  if (loadResult && loadResult.data)
    options = Object.assign(options, loadResult.data);

  // Creating settings will build the initial bars UI.
  // Changes to settings rebuild the bars.
  let barUI;
  let settingsDiv = document.getElementById('settings');
  let buildFunc = (options) => {
    while (containerDiv.lastChild)
      containerDiv.removeChild(containerDiv.lastChild);
    barUI = new BarUI(targetType, options, containerDiv);
  };
  let gSettingsUI = new SettingsUI(targetType, lang, configStructure, options, settingsDiv, buildFunc);

  window.addOverlayListener('EnmityTargetData', (e) => barUI.update(e));
  document.addEventListener('onExampleShowcase', () => showExample(barUI));
  window.startOverlayEvents();
});
