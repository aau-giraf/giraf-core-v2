import glob
import xml.etree.ElementTree as ET
from collections import defaultdict


def should_ignore(filename):
    normalized = filename.replace("\\", "/").lower()
    parts = normalized.strip("/").split("/")

    return any(part in {"obj", "bin", "migrations"} for part in parts)


def percentage(covered, total):
    return f"{100 * covered / total:.1f}%" if total else "N/A"


reports = glob.glob(
    "coverage/**/coverage.cobertura.xml",
    recursive=True,
)

if not reports:
    raise SystemExit("No Cobertura coverage reports found")


# Aggregate line coverage by source file across test projects.
files = defaultdict(lambda: {"lines": {}})

for report in reports:
    root = ET.parse(report).getroot()

    for cls in root.findall(".//class"):
        filename = cls.get("filename")

        if not filename or should_ignore(filename):
            continue

        entry = files[filename]

        for line in cls.findall("./lines/line"):
            number = line.get("number")
            hits = int(line.get("hits", "0"))

            # Merge duplicate line entries across reports/classes.
            entry["lines"][number] = max(
                entry["lines"].get(number, 0),
                hits,
            )


# Calculate coverage for each file.
rows = []

for filename, data in sorted(files.items()):
    lines = data["lines"]
    covered_lines = sum(hits > 0 for hits in lines.values())
    total_lines = len(lines)

    rows.append(
        (
            filename,
            percentage(covered_lines, total_lines),
            covered_lines,
            total_lines,
        )
    )


# Generate the Markdown report for the GitHub PR comment.
COVERAGE_THRESHOLD = 70.0

with open(
    "code-coverage-results.md",
    "w",
    encoding="utf-8",
) as out:
    out.write("## Code coverage by file\n\n")
    out.write("| Status | Filename | Line coverage |\n")
    out.write("|---|---|---:|\n")

    for filename, line_pct, covered, total in rows:
        coverage = float(line_pct.rstrip("%")) if line_pct != "N/A" else None

        if coverage is None:
            status = "⚪"
        elif coverage >= COVERAGE_THRESHOLD:
            status = "✅"
        else:
            status = "❌"

        out.write(f"| {status} | `{filename}` | {line_pct} |\n")

    if rows:
        out.write(
            f"\n<sub>Threshold: {COVERAGE_THRESHOLD:.0f}% line coverage. "
            "Excludes obj, bin, and src/migrations.</sub>\n"
        )
    else:
        out.write("No source files found after applying exclusions.\n")
