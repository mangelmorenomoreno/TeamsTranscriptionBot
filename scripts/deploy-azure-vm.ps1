$rg="rg-transcription-bot"
$loc="eastus"
$vm="vm-transcription-bot"
az group create -n $rg -l $loc
az vm create -g $rg -n $vm --image Win2022Datacenter --size Standard_D4s_v5 --admin-username azureuser --generate-ssh-keys --public-ip-sku Standard
# Abre HTTPS webhook + media control
az vm open-port -g $rg -n $vm --port 443
az vm open-port -g $rg -n $vm --port 8445
# Rango de audio (ajústalo a tu NSG)
for ($p=50000; $p -le 50019; $p++) { az vm open-port -g $rg -n $vm --port $p --protocol Udp | Out-Null }
