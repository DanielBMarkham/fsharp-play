#!/usr/bin/env bash

# script to sort dot diagrams to drive automated biz dependency testing

# what are the lines with business dependencies?
grep '\->' VeterinaryClinic.pu | grep '\[color=red'

# what are the lines with Process Area Business dependencies?
grep '\->' VeterinaryClinic.pu | grep '\[color=red'| grep 'lhead'

# what are the left/right for any connection?
# grep '\->' VeterinaryClinic.pu | grep '\[color=red'| grep 'lhead' | grep -oEi '[a-z]+'

# left pa deps including arrow
grep '\->' VeterinaryClinic.pu | grep '\[color=red'| grep 'lhead' | grep -oEi '[a-z|A-Z|0-9]+\->'

# right pa deps including arrow
grep '\->' VeterinaryClinic.pu | grep '\[color=red'| grep 'lhead' | grep -oEi '\->[a-z|A-Z|0-9]+'

# non pa deps
grep '\->' VeterinaryClinic.pu | grep '\[color=red'| grep -v 'lhead'