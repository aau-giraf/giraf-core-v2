import glob
import xml.etree.ElementTree as ET
from collections import defaultdict

reports = glob.glob("coverage/**/coverage.cobertura.xml", recursive=True)
if not reports:
    raise SystemExit("No Cobertura coverage reports found")

# Aggregate coverage by source file across test projects.
files = defaultdict(lambda: {"lines": {}, "branches": {}})

for report in reports:
    root = ET.parse(report).getroot()

    for cls in root.findall(".//class"):
        filename = cls.get("filename")
        if not filename:
            continue

        entry = files[filename]

        for line in cls.findall("./lines/line"):
            number = line.get("number")
            hits = int(line.get("hits", "0"))
            key = (number, line.get("branch", "false"))
            entry["lines"][key] = max(entry["lines"].get(key, 0), hits)

            if line.get("branch") == "true":
                for condition in line.findall("./conditions/condition"):
                    branch_key = (number, condition.get("number"))
                    covered = int(condition.get("coverage", "0").rstrip("%") != "0")
                    entry["branches"][branch_key] = max(
                        entry["branches"].get(branch_key, 0), covered
                    )


def percentage(covered, total):
    return f"{100 * covered / total:.1f}%" if total else "N/A"


rows = []
for filename, data in sorted(files.items()):
    lines = data["lines"]
    covered_lines = sum(hits > 0 for hits in lines.values())
    total_lines = len(lines)

    branches = data["branches"]
    covered_branches = sum(branches.values())
    total_branches = len(branches)

    rows.append(
        (
            filename,
            percentage(covered_lines, total_lines),
            percentage(covered_branches, total_branches),
            covered_lines,
            total_lines,
        )
    )

with open("code-coverage-results.md", "w", encoding="utf-8") as out:
    out.write("## Code coverage by file\n\n")
    out.write("| File | Line coverage | Branch coverage |\n")
    out.write("|---|---:|---:|\n")

    for filename, line_pct, branch_pct, _, _ in rows:
        out.write(f"| `{filename}` | {line_pct} | {branch_pct} |\n")

    if rows:
        out.write("\n<sub>Generated from Cobertura coverage reports.</sub>\n")
