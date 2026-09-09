# Maintenance ledger

This is the active task record. The old CIM migration plan and catch audit are
historical, not an instruction to resume modernization. Remain on .NET Framework
4.8; build the checked-in original library source; make narrow, tested fixes.

## Completed

| ID | Work | Evidence |
|---|---|---|
| REC-01 | Recover Roger's original library source and preserve security fixes | 4077119; source revision and patches in sccmclictr.automation/UPSTREAM.md |
| INV-01 | Correct upstream installed-software metadata assignments | b0cc9da; WMI and serialized-CIM regression tests |
| REL-01 | Publish signed 1.3.0.2 installer and portable package | Release commit 570af2d; GitHub run 34403971376 succeeded; downloaded checksums, first-party signatures, timestamps and provenance verified |
| VAL-01 | Validate single-update installation and restart | Maintainer observed KB5126052 download, PendingSoftReboot, app-triggered restart and final Installed status |
| VAL-02 | Manual workstation checks and log-file opening | Maintainer reported available checks complete and subsequently confirmed log-file opening works; not a claim of automated full-feature parity |

## Current offline maintenance

| ID | Work | Status / evidence |
|---|---|---|
| CACHE-01 | Fix stale script-result cache and cached error records | Three new tests reproduce failures on the released library. Narrow fix plus four regression cases; verification recorded below. Unreleased. |

## Deferred / not claimed complete

| ID | Work | Boundary |
|---|---|---|
| DOMAIN-01 | Additional live parity checks, individual policy triggers and bulk update actions | Domain-context testing only when available; no further cross-domain credential prompts. No separately recorded results for those actions. |
| ERR-01 | Review remaining legacy error swallowing and return-code handling | Offline review can continue incrementally. Do not globally change error semantics or rewrite every catch without tests. |
| WARN-01 | Assess original-source compiler warnings | Prioritize evidence of behavior defects; no warning-count cleanup or version bump for its own sake. |

## Verification record

- Release 1.3.0.2: application and 14 plugins built; 12 recovery Pester tests and
  2 MSTest tests passed; 2 automated live parity tests skipped.
- CACHE-01 baseline reproduction: stale populated result, stale result after empty
  refresh, and cached initial provider error all failed against the released
  library. Preserving prior good data after failed refresh already passed.
- CACHE-01 post-fix validation: application and all 14 plugins rebuilt; 16 Pester
  tests and 2 MSTest tests passed, including all four cache cases. Maintenance
  guard and diff whitespace checks passed. Two live parity tests remained skipped;
  no remote client actions or authentication prompts were used.

Published releases are immutable for this work. Maintenance-branch changes do not
alter 1.3.0.2, and nothing here schedules another release automatically.
