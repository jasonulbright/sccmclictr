# Security policy

## Supported version

Security fixes are developed in this maintenance fork. Published v1.3.0.1 assets predate the current automation-library recovery; do not assume unreleased fixes are present in them. Old fork releases and tags were removed; upstream releases are not maintained here.

The shipping runtime target remains .NET Framework 4.8. Runtime servicing follows the supported host Windows lifecycle, not a blanket support date for every host OS or ConfigMgr version.

## Reporting a vulnerability

Do not open a public issue for a vulnerability, credential exposure, or exploit details. Use GitHub's **Report a vulnerability** option on the repository Security page. Include affected versions, reproduction steps, impact, and any suggested mitigation, with environment details sanitized.

Client Center executes administrative operations over WinRM and is intended for trusted administrators on managed internal networks. Reports involving credential handling, command construction, TLS validation, release signing, or unsafe remote operations are especially useful.
