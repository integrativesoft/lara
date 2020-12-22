/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

require("webpack-jquery-ui/autocomplete")
require("webpack-jquery-ui/css")

import { getDocumentId } from "./index"

export interface AutocompleteCommand {
  ElementId: string
  AutoFocus: boolean
  MinLength: number
  Strict: boolean
}

interface AutocompleteEntry {
  html?: string
  label: string
  subtitle?: string
  code: string
}

interface SourceRequest {
  term: string
}

interface AutocompleteRequest {
  Key: string
  Term: string
}

type AutocompleteCallback = (_list: AutocompleteEntry[]) => void

interface SuccessData {
  Suggestions: AutocompleteEntry[]
}

export function autocompleteStart(json: string): void {
  const step = JSON.parse(json) as AutocompleteCommand
  const input = document.getElementById(step.ElementId) as HTMLInputElement
  $(input).autocomplete({
    autoFocus: step.AutoFocus,
    minLength: step.MinLength,
    source: function (
      request: SourceRequest,
      updater: AutocompleteCallback
    ): void {
      $.ajax({
        url: "/lara_autocomplete",
        dataType: "json",
        data: buildData(input, request.term),
        success: function (data: SuccessData) {
          updater(data.Suggestions)
        },
        method: "POST"
      })
    },
    select: function (_event, ui: JQueryUI.AutocompleteUIParams): void {
      const entry = ui.item as AutocompleteEntry
      setValue(input, entry.code, entry.label)
    },
    change: function (_event, ui: JQueryUI.AutocompleteUIParams): void {
      if (step.Strict && !ui.item) {
        setValue(input, "", "")
      }
    }
  })
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const instance = $(input).autocomplete("instance") as any
  instance._renderItem = render
}

export function autocompleteStop(id: string): void {
  const input = document.getElementById(id)
  $(input).autocomplete("destroy")
}

function setValue(input: HTMLInputElement, value: string, text: string): void {
  if (!value) {
    input.removeAttribute("data-lara-value")
  } else {
    input.setAttribute("data-lara-value", value)
  }
  input.value = text
}

function render(ul: Element, entry: AutocompleteEntry): JQuery<HTMLElement> {
  let html: string
  if (entry.html) {
    html = entry.html
  } else {
    html = buildLabel(entry)
  }
  return $("<li>").append(html).appendTo(ul)
}

function buildLabel(entry: AutocompleteEntry): string {
  const div = $("<div>", { class: "autocompleteEntry" })
  const title = $("<span>", { class: "autocompleteTitle" })
  title.text(entry.label)
  div.append(title)
  if (entry.subtitle) {
    const subtitle = $('<span style="font-size:small;">', {
      class: "autocompleteSubtitle"
    })
    subtitle.text(entry.subtitle)
    div.append("<br>")
    div.append(subtitle)
  }
  return div.clone().wrap("<p>").parent().html()
}

function buildData(input: HTMLInputElement, term: string): string {
  return JSON.stringify(buildRequest(input, term))
}

function buildRequest(
  input: HTMLInputElement,
  term: string
): AutocompleteRequest {
  const documentId = getDocumentId()
  const key = input.id + " " + documentId
  return {
    Key: key,
    Term: term
  }
}
